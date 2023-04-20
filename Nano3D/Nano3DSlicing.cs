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
            // TODO: complete command.
            return Result.Success;
        }
    }
}