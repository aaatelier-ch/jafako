using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using VisualARQMac.Objects;

namespace VisualARQMac.Commands
{
    /// <summary>VA_Stair - create a parametric straight-flight stair.</summary>
    public class CreateStairCommand : Command
    {
        public override string EnglishName => "VA_Stair";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Point3d start = Point3d.Origin;
            if (RhinoGet.GetPoint("Stair base point", false, ref start) != Result.Success)
                return Result.Cancel;

            Point3d end = start + Vector3d.XAxis;
            if (RhinoGet.GetPoint("Stair run direction point", false, ref end) != Result.Success)
                return Result.Cancel;

            double totalHeight = 3.0;
            int steps = 16;
            if (RhinoGet.GetNumber("Floor-to-floor height", false, ref totalHeight) != Result.Success) return Result.Cancel;
            if (RhinoGet.GetInteger("Number of steps", false, ref steps) != Result.Success) return Result.Cancel;

            var stair = new Stair { BasePoint = start, Direction = end - start };
            stair.Parameters["TotalHeight"].Value = totalHeight;
            stair.Parameters["StepCount"].Value = steps;
            stair.Update();

            RhinoApp.WriteLine($"Created stair {stair.Id}.");
            return Result.Success;
        }
    }
}
