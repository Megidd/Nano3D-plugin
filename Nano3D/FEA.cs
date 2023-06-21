using System;
using Rhino;
using Rhino.Commands;

namespace Nano3D
{
    public class FEA : Command
    {
        public FEA()
        {
            Instance = this;
        }

        ///<summary>The only instance of the MyCommand command.</summary>
        public static FEA Instance { get; private set; }

        public override string EnglishName => "Finite Element Analysis";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: complete command.
            return Result.Success;
        }
    }
}