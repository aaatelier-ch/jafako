using Rhino;
using Rhino.Commands;
using Rhino.Input;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Commands
{
    /// <summary>VA_ImportIFC - import BIM objects from an IFC file.</summary>
    public class ImportIfcCommand : Command
    {
        public override string EnglishName => "VA_ImportIFC";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            string path = string.Empty;
            if (RhinoGet.GetString("IFC file to import", false, ref path) != Result.Success || string.IsNullOrWhiteSpace(path))
                return Result.Cancel;

            if (!System.IO.File.Exists(path))
            {
                RhinoApp.WriteLine($"File not found: {path}");
                return Result.Failure;
            }

            var importer = new IfcImporter();
            var created = importer.Import(path);

            RhinoApp.WriteLine($"Imported {created.Count} object(s) from {path}.");
            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
