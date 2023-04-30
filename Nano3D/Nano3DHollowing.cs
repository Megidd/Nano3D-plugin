using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

            // TODO: complete command.

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

            MeshHelper.SaveBuffersAsStl(vertexBuffer, indexBuffer, "mesh-out.stl");

            byte[] data = MeshHelper.ProcessBuffers(indexBuffer, vertexBuffer);

            Dictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("thickness", "0.5");
            fields.Add("precision", "3");
            fields.Add("infill", "false");
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            files.Add("input", data);
            string response = HttpHelper.SendHttpPostRequest(HttpHelper.UrlHollowing, fields, files);
            Console.WriteLine(response);

            RhinoApp.WriteLine("The {0} command finished.", EnglishName);
            return Result.Success;
        }
    }
}