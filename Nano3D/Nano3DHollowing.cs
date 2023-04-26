using System;
using System.Net.Http;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input;
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

            const ObjectType geometryFilter = ObjectType.Mesh;
            int integer1 = 300;
            int integer2 = 300;

            OptionInteger optionInteger1 = new OptionInteger(integer1, 200, 900);
            OptionInteger optionInteger2 = new OptionInteger(integer2, 200, 900);

            GetObject go = new GetObject();
            go.SetCommandPrompt("Select a mesh");
            go.GeometryFilter = geometryFilter;
            go.AddOptionInteger("Option1", ref optionInteger1);
            go.AddOptionInteger("Option2", ref optionInteger2);
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;

            bool bHavePreselectedObjects = false;

            for (; ; )
            {
                GetResult res = go.GetMultiple(1, 0);

                if (res == GetResult.Option)
                {
                    go.EnablePreSelect(false, true);
                    continue;
                }

                else if (res != GetResult.Object)
                    return Result.Cancel;

                if (go.ObjectsWerePreselected)
                {
                    bHavePreselectedObjects = true;
                    go.EnablePreSelect(false, true);
                    continue;
                }

                break;
            }

            if (bHavePreselectedObjects)
            {
                // Normally, pre-selected objects will remain selected, when a
                // command finishes, and post-selected objects will be unselected.
                // This this way of picking, it is possible to have a combination
                // of pre-selected and post-selected. So, to make sure everything
                // "looks the same", lets unselect everything before finishing
                // the command.
                for (int i = 0; i < go.ObjectCount; i++)
                {
                    RhinoObject rhinoObject = go.Object(i).Object();
                    if (null != rhinoObject)
                        rhinoObject.Select(false);
                }
                doc.Views.Redraw();
            }

            int objectCount = go.ObjectCount;
            integer1 = optionInteger1.CurrentValue;
            integer2 = optionInteger2.CurrentValue;

            RhinoApp.WriteLine(string.Format("Select object count = {0}", objectCount));
            RhinoApp.WriteLine(string.Format("Value of integer1 = {0}", integer1));
            RhinoApp.WriteLine(string.Format("Value of integer2 = {0}", integer2));

            RhinoApp.WriteLine("The {0} command finished.", EnglishName);

            return Result.Success;
        }
    }
}