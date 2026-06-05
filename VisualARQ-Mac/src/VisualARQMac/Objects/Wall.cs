using System;
using Rhino.Geometry;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Objects
{
    /// <summary>Represents a parametric wall object.</summary>
    public class Wall : BimObject
    {
        /// <summary>Wall path (base curve in the world XY plane).</summary>
        public Curve Path { get; set; }

        /// <summary>Optional explicit cross-section profile. When null one is built
        /// from <see cref="Thickness"/> and <see cref="Height"/>.</summary>
        public Curve Profile { get; set; }

        public double Height
        {
            get => Parameters.GetValue<double>("Height");
            set => SetParameter("Height", value);
        }

        public double Thickness
        {
            get => Parameters.GetValue<double>("Thickness");
            set => SetParameter("Thickness", value);
        }

        public override string ObjectType => "Wall";
        public override string IfcType => "IfcWall";

        public Wall() : base(BimDocument.GetOrCreateStyle("Default Wall"))
        {
            Name = "Wall";
            Layer = "Walls";

            Parameters.Add(new BimParameter("Height", 3.0, "Wall height in meters") { Unit = "m", IfcProperty = "OverallHeight" });
            Parameters.Add(new BimParameter("Thickness", 0.2, "Wall thickness in meters") { Unit = "m", IfcProperty = "Width" });
        }

        public override GeometryBase CreateGeometry()
        {
            if (Path == null)
                return null;

            double thickness = Thickness <= 0 ? 0.2 : Thickness;
            double height = Height <= 0 ? 3.0 : Height;

            // Offset the path to both sides to obtain a closed footprint, then
            // extrude it vertically to form the wall solid.
            var plane = Plane.WorldXY;
            var left = Path.Offset(plane, thickness / 2.0, 0.001, CurveOffsetCornerStyle.Sharp);
            var right = Path.Offset(plane, -thickness / 2.0, 0.001, CurveOffsetCornerStyle.Sharp);
            if (left == null || left.Length == 0 || right == null || right.Length == 0)
                return Extrude(Path, thickness, height);

            var footprint = JoinFootprint(left[0], right[0]);
            if (footprint == null || !footprint.IsClosed)
                return null;

            var extrusion = Extrusion.Create(footprint, height, true);
            return extrusion?.ToBrep();
        }

        private static Curve JoinFootprint(Curve left, Curve right)
        {
            right.Reverse();
            var pieces = new[]
            {
                left,
                new LineCurve(left.PointAtEnd, right.PointAtStart),
                right,
                new LineCurve(right.PointAtEnd, left.PointAtStart)
            };
            var joined = Curve.JoinCurves(pieces, 0.001);
            return joined != null && joined.Length > 0 ? joined[0] : null;
        }

        public override void ExportToIfc(IfcExporter exporter)
        {
            int productId = exporter.AddRecord(
                $"IFCWALL('{Id:N}',$,'{Name}',$,$,$,$,$,.NOTDEFINED.)");
            exporter.AddPropertySet(productId, this);
        }

        public override void ImportFromIfc(object ifcEntity)
        {
            // The PoC importer reconstructs walls from default parameters; a name is
            // recovered from the raw STEP record when present.
            if (ifcEntity is string raw)
            {
                var name = ExtractName(raw);
                if (!string.IsNullOrEmpty(name)) Name = name;
            }
        }

        private static string ExtractName(string raw)
        {
            // IFCWALL('guid',$,'Name',...) -> third quoted argument.
            int count = 0, start = -1;
            for (int i = 0; i < raw.Length; i++)
            {
                if (raw[i] != '\'') continue;
                count++;
                if (count == 5) start = i + 1;
                else if (count == 6 && start >= 0) return raw.Substring(start, i - start);
            }
            return null;
        }

        public override BimObject Clone()
        {
            var clone = new Wall
            {
                Name = Name,
                Layer = Layer,
                Style = Style,
                Path = Path?.DuplicateCurve(),
                Profile = Profile?.DuplicateCurve()
            };
            clone.Parameters.Clear();
            foreach (var p in Parameters) clone.Parameters.Add(p.Clone());
            return clone;
        }
    }
}
