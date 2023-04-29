using System;
using System.Net.Http;
using Eto.Forms;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
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

            RhinoObject obj = GetSingleMesh();
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
            MeshFaceList faces = mesh.Faces;
            bool converted = faces.ConvertQuadsToTriangles();
            if (converted)
            {
                RhinoApp.WriteLine("Cannot convert quads to triangles. Maybe there is no quad already.");
            }
            RhinoApp.WriteLine("Number of mesh triangles is {0}.", faces.Count);

            RhinoApp.WriteLine("The {0} command finished.", EnglishName);
            return Result.Success;
        }

        // A simple method to prompt the user to select a single mesh and return it.
        public static RhinoObject GetSingleMesh()
        {
            GetObject go = new GetObject();
            go.SetCommandPrompt("Select a single mesh");
            go.GeometryFilter = ObjectType.Mesh;
            go.Get();
            if (go.CommandResult() != Result.Success) return null;
            if (go.ObjectCount != 1) return null;
            RhinoObject obj = go.Object(0).Object();
            if (obj.ObjectType != ObjectType.Mesh) return null;
            return obj;
        }
    }
}