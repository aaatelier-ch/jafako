using System;
using Rhino;
using Rhino.Geometry;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Objects
{
    /// <summary>Represents a parametric single-slope (shed) roof.</summary>
    public class Roof : BimObject
    {
        /// <summary>Closed boundary curve of the roof footprint in plan.</summary>
        public Curve Boundary { get; set; }

        /// <summary>Base elevation of the low eave.</summary>
        public double Elevation { get; set; }

        /// <summary>Slope of the roof in degrees.</summary>
        public double SlopeDegrees
        {
            get => Parameters.GetValue<double>("Slope");
            set => SetParameter("Slope", value);
        }

        public double Thickness
        {
            get => Parameters.GetValue<double>("Thickness");
            set => SetParameter("Thickness", value);
        }

        public override string ObjectType => "Roof";
        public override string IfcType => "IfcRoof";

        public Roof() : base(BimDocument.GetOrCreateStyle("Default Wall"))
        {
            Name = "Roof";
            Layer = "Roofs";

            Parameters.Add(new BimParameter("Slope", 30.0, "Roof slope in degrees") { Unit = "deg", Type = ParameterType.Angle });
            Parameters.Add(new BimParameter("Thickness", 0.3, "Roof thickness in meters") { Unit = "m" });
        }

        public override GeometryBase CreateGeometry()
        {
            if (Boundary == null || !Boundary.IsClosed)
                return null;

            double thickness = Thickness <= 0 ? 0.3 : Thickness;
            double slope = Math.Max(0.0, Math.Min(89.0, SlopeDegrees));

            var bbox = Boundary.GetBoundingBox(true);
            double run = bbox.Max.Y - bbox.Min.Y;
            double rise = run * Math.Tan(RhinoMath.ToRadians(slope));

            // Shear the footprint along +Y so it rises with the requested slope,
            // place it at the base elevation, then thicken into a solid.
            var curve = Boundary.DuplicateCurve();
            var shear = ShearTransform(bbox.Min.Y, rise / Math.Max(run, 1e-9));
            curve.Transform(shear);
            curve.Translate(new Vector3d(0, 0, Elevation));

            var extrusion = Extrusion.Create(curve, thickness, true);
            return extrusion?.ToBrep();
        }

        private static Transform ShearTransform(double baseY, double slopePerUnit)
        {
            // Maps each point (x, y, z) -> (x, y, z + (y - baseY) * slopePerUnit),
            // so the eave line (y == baseY) stays at z == 0 while the ridge rises.
            // Row-major: z' = M20*x + M21*y + M22*z + M23.
            var t = Transform.Identity;
            t.M21 = slopePerUnit;             // dz/dy
            t.M23 = -slopePerUnit * baseY;    // keep the eave at z == 0
            return t;
        }

        public override void ExportToIfc(IfcExporter exporter)
        {
            int productId = exporter.AddRecord(
                $"IFCROOF('{Id:N}',$,'{Name}',$,$,$,$,$,.SHED_ROOF.)");
            exporter.AddPropertySet(productId, this);
        }

        public override void ImportFromIfc(object ifcEntity) { }

        public override BimObject Clone()
        {
            var clone = new Roof
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
