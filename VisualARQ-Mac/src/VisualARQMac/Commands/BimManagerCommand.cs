using System.Linq;
using Rhino;
using Rhino.Commands;
using VisualARQMac.Core;

namespace VisualARQMac.Commands
{
    /// <summary>
    /// VA_Manager - report the current BIM model contents.
    /// </summary>
    /// <remarks>
    /// A full implementation would open a dockable Eto.Forms panel listing
    /// objects and their parameters. This PoC prints a summary to the command
    /// line so the workflow can be exercised without a UI layer.
    /// </remarks>
    public class BimManagerCommand : Command
    {
        public override string EnglishName => "VA_Manager";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var objects = BimDocument.GetAllObjects().ToList();
            RhinoApp.WriteLine("VisualARQ-Mac BIM Manager");
            RhinoApp.WriteLine("=========================");
            RhinoApp.WriteLine($"Objects: {objects.Count}");

            foreach (var group in objects.GroupBy(o => o.ObjectType).OrderBy(g => g.Key))
                RhinoApp.WriteLine($"  {group.Key}: {group.Count()}");

            RhinoApp.WriteLine($"Styles: {BimDocument.GetAllStyles().Count()}");
            foreach (var style in BimDocument.GetAllStyles())
                RhinoApp.WriteLine($"  {style.Name} ({style.MaterialName})");

            return Result.Success;
        }
    }
}
