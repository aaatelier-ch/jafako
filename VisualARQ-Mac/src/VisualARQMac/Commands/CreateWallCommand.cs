using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using VisualARQMac.Objects;

namespace VisualARQMac.Commands
{
    /// <summary>VA_Wall - create a parametric wall along a base curve.</summary>
    public class CreateWallCommand : Command
    {
        public override string EnglishName => "VA_Wall";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var gc = new GetObject();
            gc.SetCommandPrompt("Select a curve for the wall path");
            gc.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gc.Get();
            if (gc.CommandResult() != Result.Success)
                return gc.CommandResult();

            var path = gc.Object(0).Curve();
            if (path == null)
                return Result.Failure;

            double height = 3.0, thickness = 0.2;
            if (RhinoGet.GetNumber("Wall height", false, ref height) != Result.Success) return Result.Cancel;
            if (RhinoGet.GetNumber("Wall thickness", false, ref thickness) != Result.Success) return Result.Cancel;

            var wall = new Wall { Path = path.DuplicateCurve() };
            wall.Parameters["Height"].Value = height;
            wall.Parameters["Thickness"].Value = thickness;
            wall.Update();

            RhinoApp.WriteLine($"Created wall {wall.Id}.");
            return Result.Success;
        }
    }
}
