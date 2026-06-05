using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using VisualARQMac.Objects;

namespace VisualARQMac.Commands
{
    /// <summary>VA_Roof - create a parametric single-slope roof from a boundary.</summary>
    public class CreateRoofCommand : Command
    {
        public override string EnglishName => "VA_Roof";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var gc = new GetObject();
            gc.SetCommandPrompt("Select a closed curve for the roof footprint");
            gc.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gc.Get();
            if (gc.CommandResult() != Result.Success)
                return gc.CommandResult();

            var boundary = gc.Object(0).Curve();
            if (boundary == null || !boundary.IsClosed)
            {
                RhinoApp.WriteLine("The roof footprint curve must be closed.");
                return Result.Failure;
            }

            double slope = 30.0, thickness = 0.3;
            if (RhinoGet.GetNumber("Roof slope (degrees)", false, ref slope) != Result.Success) return Result.Cancel;
            if (RhinoGet.GetNumber("Roof thickness", false, ref thickness) != Result.Success) return Result.Cancel;

            var roof = new Roof { Boundary = boundary.DuplicateCurve() };
            roof.Parameters["Slope"].Value = slope;
            roof.Parameters["Thickness"].Value = thickness;
            roof.Update();

            RhinoApp.WriteLine($"Created roof {roof.Id}.");
            return Result.Success;
        }
    }
}
