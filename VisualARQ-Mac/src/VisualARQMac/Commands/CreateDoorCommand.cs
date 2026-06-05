using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using VisualARQMac.Objects;

namespace VisualARQMac.Commands
{
    /// <summary>VA_Door - place a parametric door at a point.</summary>
    public class CreateDoorCommand : Command
    {
        public override string EnglishName => "VA_Door";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Point3d insert = Point3d.Origin;
            if (RhinoGet.GetPoint("Door insertion point", false, ref insert) != Result.Success)
                return Result.Cancel;

            double width = 0.9, height = 2.1;
            if (RhinoGet.GetNumber("Door width", false, ref width) != Result.Success) return Result.Cancel;
            if (RhinoGet.GetNumber("Door height", false, ref height) != Result.Success) return Result.Cancel;

            var door = new Door { BasePlane = new Plane(insert, Vector3d.ZAxis) };
            door.Parameters["Width"].Value = width;
            door.Parameters["Height"].Value = height;
            door.Update();

            RhinoApp.WriteLine($"Created door {door.Id}.");
            return Result.Success;
        }
    }
}
