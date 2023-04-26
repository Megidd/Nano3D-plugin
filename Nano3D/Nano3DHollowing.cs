using System;
using System.Net.Http;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input.Custom;

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

            var response = client.GetStringAsync("http://127.0.0.1:8080/status").Result;

            if (response != null)
            {
                RhinoApp.WriteLine("Nano3D server responded with status: {0}", response);
            }

            // TODO: complete command.

            Guid meshId = GetObjectId();
            RhinoApp.WriteLine("Mesh {0} is selected.", meshId);

            RhinoApp.WriteLine("The {0} command finished.", EnglishName);

            return Result.Success;
        }

        // A simple method to select a single mesh and leave other selected objects selected.
        // https://discourse.mcneel.com/t/getobject-only-one-with-enter-prompt/111842/2?u=megidd_git
        public Guid GetObjectId()
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