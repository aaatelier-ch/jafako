using System;
using System.Collections.Generic;
using Rhino.Geometry;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Objects
{
    /// <summary>Represents a parametric straight-flight staircase.</summary>
    public class Stair : BimObject
    {
        /// <summary>Start point of the flight, at the bottom step nosing.</summary>
        public Point3d BasePoint { get; set; } = Point3d.Origin;

        /// <summary>Horizontal direction the flight runs in (XY).</summary>
        public Vector3d Direction { get; set; } = Vector3d.XAxis;

        public double TotalHeight
        {
            get => Parameters.GetValue<double>("TotalHeight");
            set => SetParameter("TotalHeight", value);
        }

        public double Width
        {
            get => Parameters.GetValue<double>("Width");
            set => SetParameter("Width", value);
        }

        public int StepCount
        {
            get => Parameters.GetValue<int>("StepCount");
            set => SetParameter("StepCount", value);
        }

        public double Tread
        {
            get => Parameters.GetValue<double>("Tread");
            set => SetParameter("Tread", value);
        }

        public override string ObjectType => "Stair";
        public override string IfcType => "IfcStair";

        public Stair() : base(BimDocument.GetOrCreateStyle("Default Wall"))
        {
            Name = "Stair";
            Layer = "Stairs";

            Parameters.Add(new BimParameter("TotalHeight", 3.0, "Floor-to-floor height in meters") { Unit = "m" });
            Parameters.Add(new BimParameter("Width", 1.0, "Stair width in meters") { Unit = "m" });
            Parameters.Add(new BimParameter("StepCount", 16, "Number of risers"));
            Parameters.Add(new BimParameter("Tread", 0.28, "Tread depth ('going') in meters") { Unit = "m" });
        }

        public override GeometryBase CreateGeometry()
        {
            int steps = StepCount <= 0 ? 16 : StepCount;
            double totalHeight = TotalHeight <= 0 ? 3.0 : TotalHeight;
            double width = Width <= 0 ? 1.0 : Width;
            double tread = Tread <= 0 ? 0.28 : Tread;
            double riser = totalHeight / steps;

            var dir = Direction;
            dir.Z = 0;
            if (!dir.Unitize()) dir = Vector3d.XAxis;
            var side = Vector3d.CrossProduct(dir, Vector3d.ZAxis);
            side.Unitize();

            var breps = new List<Brep>();
            for (int i = 0; i < steps; i++)
            {
                var origin = BasePoint + dir * (tread * i) + side * (-width / 2.0);
                var plane = new Plane(origin, dir, side);
                var box = new Box(plane,
                    new Interval(0, tread),
                    new Interval(0, width),
                    new Interval(0, riser * (i + 1)));
                var brep = box.ToBrep();
                if (brep != null) breps.Add(brep);
            }

            if (breps.Count == 0) return null;
            var union = Brep.CreateBooleanUnion(breps, 0.001);
            if (union != null && union.Length > 0) return union[0];

            // If the boolean fails, return the steps as a joined non-manifold brep.
            var joined = new Brep();
            foreach (var b in breps) joined.Append(b);
            return joined;
        }

        public override void ExportToIfc(IfcExporter exporter)
        {
            int productId = exporter.AddRecord(
                $"IFCSTAIR('{Id:N}',$,'{Name}',$,$,$,$,$,.STRAIGHT_RUN_STAIR.)");
            exporter.AddPropertySet(productId, this);
        }

        public override void ImportFromIfc(object ifcEntity) { }

        public override BimObject Clone()
        {
            var clone = new Stair { Name = Name, Layer = Layer, Style = Style, BasePoint = BasePoint, Direction = Direction };
            clone.Parameters.Clear();
            foreach (var p in Parameters) clone.Parameters.Add(p.Clone());
            return clone;
        }
    }
}
