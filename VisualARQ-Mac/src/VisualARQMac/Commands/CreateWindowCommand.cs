using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using VisualARQMac.Objects;

namespace VisualARQMac.Commands
{
    /// <summary>VA_Window - place a parametric window at a point.</summary>
    public class CreateWindowCommand : Command
    {
        public override string EnglishName => "VA_Window";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Point3d insert = Point3d.Origin;
            if (RhinoGet.GetPoint("Window insertion point", false, ref insert) != Result.Success)
                return Result.Cancel;

            double width = 1.2, height = 1.2, sill = 0.9;
            if (RhinoGet.GetNumber("Window width", false, ref width) != Result.Success) return Result.Cancel;
            if (RhinoGet.GetNumber("Window height", false, ref height) != Result.Success) return Result.Cancel;
            if (RhinoGet.GetNumber("Sill height", false, ref sill) != Result.Success) return Result.Cancel;

            var window = new Window { BasePlane = new Plane(insert, Vector3d.ZAxis) };
            window.Parameters["Width"].Value = width;
            window.Parameters["Height"].Value = height;
            window.Parameters["SillHeight"].Value = sill;
            window.Update();

            RhinoApp.WriteLine($"Created window {window.Id}.");
            return Result.Success;
        }
    }
}
