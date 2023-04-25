using System;
using System.Net.Http;
using Rhino;
using Rhino.Commands;

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

            RhinoApp.WriteLine("The {0} command finished.", EnglishName);

            return Result.Success;
        }
    }
}