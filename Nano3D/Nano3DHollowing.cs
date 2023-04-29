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

            Guid meshId = GetMeshId();
            RhinoApp.WriteLine("Mesh {0} is selected.", meshId);
            var obj = doc.Objects.FindId(meshId);
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

        // A simple method to select a single mesh and leave other selected objects selected.
        // https://discourse.mcneel.com/t/getobject-only-one-with-enter-prompt/111842/2?u=megidd_git
        public Guid GetMeshId()
        {
            var rc = Guid.Empty;
            var go = new GetObject();
            go.SetCommandPrompt("Select mesh");
            go.GeometryFilter = ObjectType.Mesh;
            go.EnablePreSelect(false, true);
            go.DeselectAllBeforePostSelect = false;
            go.Get();
            if (go.CommandResult() == Result.Success)
            {
                var rh_obj = go.Object(0).Object();
                if (null != rh_obj)
                {
                    rh_obj.Select(true); // leave selected
                    rc = rh_obj.Id;
                }
            }
            return rc;
        }
    }
}