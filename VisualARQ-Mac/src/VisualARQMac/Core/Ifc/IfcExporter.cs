using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using VisualARQMac.Core;

namespace VisualARQMac.Core.Ifc
{
    /// <summary>
    /// Minimal IFC (ISO-10303-21 / STEP) exporter.
    /// </summary>
    /// <remarks>
    /// This is a self-contained text writer producing an IFC4 file with a single
    /// building storey. It does not depend on IfcOpenShell so the proof-of-concept
    /// stays buildable; geometry is emitted as <c>IfcBuildingElementProxy</c>
    /// placeholders with the object's property set. Swap in IfcOpenShell bindings
    /// for production-grade geometry.
    /// </remarks>
    public class IfcExporter
    {
        private int _nextId = 1;

        /// <summary>Records emitted in the DATA section, keyed by their #id.</summary>
        private readonly List<string> _records = new List<string>();

        /// <summary>Reserve and return the next STEP entity id.</summary>
        public int NextId() => _nextId++;

        /// <summary>Append a raw STEP record, returning its assigned id.</summary>
        public int AddRecord(string ifcEntity)
        {
            int id = NextId();
            _records.Add($"#{id}= {ifcEntity};");
            return id;
        }

        /// <summary>Format a property set for a single BIM object.</summary>
        public void AddPropertySet(int productId, BimObject obj)
        {
            var props = new List<int>();
            foreach (var p in obj.Parameters)
            {
                string value = Convert.ToString(p.Value, CultureInfo.InvariantCulture);
                props.Add(AddRecord(
                    $"IFCPROPERTYSINGLEVALUE('{Escape(p.Name)}',$,IFCTEXT('{Escape(value)}'),$)"));
            }

            if (props.Count == 0) return;

            int psetId = AddRecord(
                $"IFCPROPERTYSET('{Guid.NewGuid():N}',$,'VisualARQ-Mac',$,({Refs(props)}))");
            AddRecord(
                $"IFCRELDEFINESBYPROPERTIES('{Guid.NewGuid():N}',$,$,$,(#{productId}),#{psetId})");
        }

        /// <summary>Export a collection of BIM objects to an IFC file on disk.</summary>
        public void Export(IEnumerable<BimObject> objects, string filePath)
        {
            _records.Clear();
            _nextId = 1;

            // Project / context scaffolding.
            int projectId = AddRecord(
                $"IFCPROJECT('{Guid.NewGuid():N}',$,'VisualARQ-Mac Project',$,$,$,$,$,$)");

            foreach (var obj in objects)
            {
                obj.ExportToIfc(this);
                // ExportToIfc implementations call back into AddRecord/AddPropertySet.
            }

            File.WriteAllText(filePath, BuildFile(), new UTF8Encoding(false));
        }

        private string BuildFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("ISO-10303-21;");
            sb.AppendLine("HEADER;");
            sb.AppendLine("FILE_DESCRIPTION(('ViewDefinition [CoordinationView]'),'2;1');");
            sb.AppendLine(
                $"FILE_NAME('export.ifc','{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss}',(''),(''),'VisualARQ-Mac','VisualARQ-Mac 1.0.0','');");
            sb.AppendLine("FILE_SCHEMA(('IFC4'));");
            sb.AppendLine("ENDSEC;");
            sb.AppendLine("DATA;");
            foreach (var record in _records)
                sb.AppendLine(record);
            sb.AppendLine("ENDSEC;");
            sb.AppendLine("END-ISO-10303-21;");
            return sb.ToString();
        }

        private static string Refs(IEnumerable<int> ids) => string.Join(",", System.Linq.Enumerable.Select(ids, i => $"#{i}"));

        private static string Escape(string value) =>
            (value ?? string.Empty).Replace("'", "''");
    }
}
