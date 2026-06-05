using Rhino.Geometry;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Objects
{
    /// <summary>Represents a parametric floor/ceiling slab.</summary>
    public class Slab : BimObject
    {
        /// <summary>Closed boundary curve of the slab in plan.</summary>
        public Curve Boundary { get; set; }

        /// <summary>Elevation of the slab's top face.</summary>
        public double Elevation { get; set; }

        public double Thickness
        {
            get => Parameters.GetValue<double>("Thickness");
            set => SetParameter("Thickness", value);
        }

        public override string ObjectType => "Slab";
        public override string IfcType => "IfcSlab";

        public Slab() : base(BimDocument.GetOrCreateStyle("Default Wall"))
        {
            Name = "Slab";
            Layer = "Slabs";

            Parameters.Add(new BimParameter("Thickness", 0.25, "Slab thickness in meters") { Unit = "m" });
        }

        public override GeometryBase CreateGeometry()
        {
            if (Boundary == null || !Boundary.IsClosed)
                return null;

            double thickness = Thickness <= 0 ? 0.25 : Thickness;

            // Place the boundary at the target elevation, then extrude downward.
            var curve = Boundary.DuplicateCurve();
            curve.Translate(new Vector3d(0, 0, Elevation));
            var extrusion = Extrusion.Create(curve, -thickness, true);
            return extrusion?.ToBrep();
        }

        public override void ExportToIfc(IfcExporter exporter)
        {
            int productId = exporter.AddRecord(
                $"IFCSLAB('{Id:N}',$,'{Name}',$,$,$,$,$,.FLOOR.)");
            exporter.AddPropertySet(productId, this);
        }

        public override void ImportFromIfc(object ifcEntity) { }

        public override BimObject Clone()
        {
            var clone = new Slab
            {
                Name = Name,
                Layer = Layer,
                Style = Style,
                Boundary = Boundary?.DuplicateCurve(),
                Elevation = Elevation
            };
            clone.Parameters.Clear();
            foreach (var p in Parameters) clone.Parameters.Add(p.Clone());
            return clone;
        }
    }
}
