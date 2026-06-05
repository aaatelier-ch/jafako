using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using VisualARQMac.Objects;

namespace VisualARQMac.Commands
{
    /// <summary>VA_Column - place a parametric column at a base point.</summary>
    public class CreateColumnCommand : Command
    {
        public override string EnglishName => "VA_Column";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Point3d basePoint = Point3d.Origin;
            if (RhinoGet.GetPoint("Column base point", false, ref basePoint) != Result.Success)
                return Result.Cancel;

            double height = 3.0, width = 0.3;
            if (RhinoGet.GetNumber("Column height", false, ref height) != Result.Success) return Result.Cancel;
            if (RhinoGet.GetNumber("Section size", false, ref width) != Result.Success) return Result.Cancel;

            var column = new Column { BasePoint = basePoint };
            column.Parameters["Height"].Value = height;
            column.Parameters["Width"].Value = width;
            column.Parameters["Depth"].Value = width;
            column.Update();

            RhinoApp.WriteLine($"Created column {column.Id}.");
            return Result.Success;
        }
    }
}
