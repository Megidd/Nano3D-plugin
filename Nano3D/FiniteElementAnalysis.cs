using System;
using Rhino;
using Rhino.Commands;

namespace Nano3D
{
    public class FiniteElementAnalysis : Command
    {
        public FiniteElementAnalysis()
        {
            Instance = this;
        }

        ///<summary>The only instance of the MyCommand command.</summary>
        public static FiniteElementAnalysis Instance { get; private set; }

        public override string EnglishName => "FiniteElementAnalysis";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: complete command.
            return Result.Success;
        }
    }
}