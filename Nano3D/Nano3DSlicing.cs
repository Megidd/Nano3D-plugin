using System;
using Rhino;
using Rhino.Commands;

namespace Nano3D
{
    public class Nano3DSlicing : Command
    {
        public Nano3DSlicing()
        {
            Instance = this;
        }

        ///<summary>The only instance of the MyCommand command.</summary>
        public static Nano3DSlicing Instance { get; private set; }

        public override string EnglishName => "Nano3DSlicing";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("The {0} command received the document.", EnglishName);

            // TODO: complete command.

            RhinoApp.WriteLine("The {0} command is not implemented yet.", EnglishName);

            return Result.Success;
        }
    }
}