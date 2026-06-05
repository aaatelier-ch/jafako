using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using VisualARQMac.Objects;

namespace VisualARQMac.Commands
{
    /// <summary>VA_Slab - create a parametric slab from a closed boundary curve.</summary>
    public class CreateSlabCommand : Command
    {
        public override string EnglishName => "VA_Slab";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var gc = new GetObject();
            gc.SetCommandPrompt("Select a closed curve for the slab boundary");
            gc.GeometryFilter = Rhino.DocObjects.ObjectType.Curve;
            gc.Get();
            if (gc.CommandResult() != Result.Success)
                return gc.CommandResult();

            var boundary = gc.Object(0).Curve();
            if (boundary == null || !boundary.IsClosed)
            {
                RhinoApp.WriteLine("The slab boundary curve must be closed.");
                return Result.Failure;
            }

            double thickness = 0.25;
            if (RhinoGet.GetNumber("Slab thickness", false, ref thickness) != Result.Success) return Result.Cancel;

            var slab = new Slab { Boundary = boundary.DuplicateCurve() };
            slab.Parameters["Thickness"].Value = thickness;
            slab.Update();

            RhinoApp.WriteLine($"Created slab {slab.Id}.");
            return Result.Success;
        }
    }
}
