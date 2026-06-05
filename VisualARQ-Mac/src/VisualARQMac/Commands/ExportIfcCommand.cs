using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Commands
{
    /// <summary>VA_ExportIFC - export all BIM objects to an IFC file.</summary>
    public class ExportIfcCommand : Command
    {
        public override string EnglishName => "VA_ExportIFC";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var objects = BimDocument.GetAllObjects().ToList();
            if (objects.Count == 0)
            {
                RhinoApp.WriteLine("There are no VisualARQ-Mac objects to export.");
                return Result.Nothing;
            }

            string path = string.Empty;
            if (RhinoGet.GetString("IFC output file path", false, ref path) != Result.Success || string.IsNullOrWhiteSpace(path))
                return Result.Cancel;

            if (!path.EndsWith(".ifc", System.StringComparison.OrdinalIgnoreCase))
                path += ".ifc";

            var exporter = new IfcExporter();
            exporter.Export(objects, path);

            RhinoApp.WriteLine($"Exported {objects.Count} object(s) to {path}.");
            return Result.Success;
        }
    }
}
