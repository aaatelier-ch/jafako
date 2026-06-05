using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using VisualARQMac.Core;
using VisualARQMac.Objects;

namespace VisualARQMac.Core.Ifc
{
    /// <summary>
    /// Minimal IFC (ISO-10303-21 / STEP) importer.
    /// </summary>
    /// <remarks>
    /// Parses the DATA section of an IFC4 file and instantiates a matching
    /// <see cref="BimObject"/> for each supported product entity. Geometry is
    /// reconstructed from default parameters rather than the IFC representation;
    /// replace with IfcOpenShell bindings for full geometric fidelity.
    /// </remarks>
    public class IfcImporter
    {
        private static readonly Regex EntityRegex =
            new Regex(@"#(?<id>\d+)\s*=\s*(?<type>IFC[A-Z0-9]+)\s*\((?<args>.*?)\);",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>Parse an IFC file and return the created BIM objects.</summary>
        public IReadOnlyList<BimObject> Import(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("IFC file not found.", filePath);

            string content = File.ReadAllText(filePath);
            var created = new List<BimObject>();

            foreach (Match match in EntityRegex.Matches(content))
            {
                string type = match.Groups["type"].Value.ToUpperInvariant();
                var bim = CreateFor(type);
                if (bim == null) continue;

                bim.ImportFromIfc(match.Value);
                bim.Update();
                created.Add(bim);
            }

            return created;
        }

        /// <summary>Map an IFC entity name to a concrete BIM object.</summary>
        private static BimObject CreateFor(string ifcType)
        {
            switch (ifcType)
            {
                case "IFCWALL":
                case "IFCWALLSTANDARDCASE": return new Wall();
                case "IFCDOOR": return new Door();
                case "IFCWINDOW": return new Window();
                case "IFCCOLUMN": return new Column();
                case "IFCSLAB": return new Slab();
                case "IFCSTAIR":
                case "IFCSTAIRFLIGHT": return new Stair();
                case "IFCROOF": return new Roof();
                default: return null; // Unsupported entity - ignored.
            }
        }
    }
}
