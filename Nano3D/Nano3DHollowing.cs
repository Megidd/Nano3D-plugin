using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.Input.Custom;
using Command = Rhino.Commands.Command;

namespace Nano3D
{
    public class Nano3DHollowing : Command
    {
        public Nano3DHollowing()
        {
            Instance = this;
        }

        ///<summary>The only instance of the MyCommand command.</summary>
        public static Nano3DHollowing Instance { get; private set; }

        public override string EnglishName => "Nano3DHollowing";

        private static readonly HttpClient client = new HttpClient();

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("The {0} command received the document.", EnglishName);

            RhinoObject obj = MeshHelper.GetSingle();
            if (null == obj || obj.ObjectType != ObjectType.Mesh)
            {
                RhinoApp.WriteLine("Mesh is not valid.");
                return Result.Failure;
            }
            Mesh mesh = obj.Geometry as Mesh;
            if (mesh == null)
            {
                RhinoApp.WriteLine("Mesh is not valid.");
                return Result.Failure;
            }
            bool converted = mesh.Faces.ConvertQuadsToTriangles();
            if (converted)
            {
                RhinoApp.WriteLine("Mesh contains quads. They are converted to triangles.");
            }
            RhinoApp.WriteLine("Number of mesh vertices: {0}", mesh.Vertices.Count);
            RhinoApp.WriteLine("Number of mesh triangles: {0}", mesh.Faces.Count);

            // Extract vertex buffer and index buffer.
            float[] vertexBuffer;
            int[] indexBuffer;
            MeshHelper.GetBuffers(mesh, out vertexBuffer, out indexBuffer);

            if (Utilities.debugMode)
                MeshHelper.SaveBuffersAsStl(vertexBuffer, indexBuffer, "mesh.stl");

            byte[] data = MeshHelper.ProcessBuffers(indexBuffer, vertexBuffer);

            // Prepare HTTP form text fields.
            Dictionary<string, string> fields = new Dictionary<string, string>();

            float thickness = Utilities.GetFloatFromUser(1.8, 0.0, 100.0, "Enter wall thickness for hollowing.");
            fields.Add("thickness", thickness.ToString());

            uint precision = Utilities.GetUint32FromUser("Enter precision: VeryLow=1, Low=2, Medium=3, High=4, VeryHigh=5", 3, 1, 5);
            switch (precision)
            {
                case 1: case 2: case 3: case 4: case 5:
                    break;
                default:
                    RhinoApp.WriteLine("Precision must be 1, 2, 3, 4, or 5 i.e. VeryLow=1, Low=2, Medium=3, High=4, VeryHigh=5");
                    return Result.Failure;
            }
            fields.Add("precision", precision.ToString());

            bool infill = Utilities.GetYesNoFromUser("Do you want infill for hollowed mesh?");
            fields.Add("infill", infill ? "true" : "false");

            // Prepare HTTP form files.
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            files.Add("input", data);

            // Send HTTP request asynchronously.
            Task<byte[]> task = HttpHelper.SendPostRequest(HttpHelper.UrlHollowing, fields, files);

            task.ContinueWith(responseTask =>
            {
                if (responseTask.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        byte[] response = responseTask.Result;

                        RhinoApp.WriteLine("HTTP response length: {0}.", response.Length);

                        int[] indexBufferOut;
                        float[] vertexBufferOut;
                        MeshHelper.UnpackBuffers(response, out indexBufferOut, out vertexBufferOut);
                        // Use the returned index and vertex buffers to create a new mesh.
                        Mesh meshOut = MeshHelper.CreateFromBuffers(vertexBufferOut, indexBufferOut);

                        if (Utilities.debugMode)
                            MeshHelper.SaveAsStl(meshOut, "mesh-hollowed.stl");

                        // Create a new object attributes with the desired name
                        ObjectAttributes attributes = new ObjectAttributes();
                        attributes.Name = "Hollowed: " + obj.Attributes.Name;

                        // Add the mesh to the document with the specified attributes
                        doc.Objects.AddMesh(meshOut, attributes);

                        // Redraw the viewports to update the display
                        doc.Views.Redraw();

                        if (doc.Objects.Delete(obj.Id, true))
                        {
                            // Good.
                        }
                        else
                        {
                            RhinoApp.WriteLine("The {0} command couldn't delete the original object.", EnglishName);
                        }

                        RhinoApp.WriteLine("The {0} command finished.", EnglishName);
                    }
                    catch (Exception ex)
                    {
                        RhinoApp.WriteLine("An error occurred while running the command {0}: {1}", EnglishName, ex.Message);
                    }
                }
                else if (responseTask.Status == TaskStatus.Faulted)
                {
                    RhinoApp.WriteLine("HTTP request failed: {0}", responseTask.Exception.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            Utilities.PrintWaitMessage(EnglishName);
            return Result.Success;
        }
    }
}