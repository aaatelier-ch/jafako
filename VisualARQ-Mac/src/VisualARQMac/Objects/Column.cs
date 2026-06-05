using System.Globalization;
using Rhino.Geometry;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Objects
{
    /// <summary>Represents a parametric structural column.</summary>
    public class Column : BimObject
    {
        /// <summary>Base point of the column.</summary>
        public Point3d BasePoint { get; set; } = Point3d.Origin;

        public double Height
        {
            get => Parameters.GetValue<double>("Height");
            set => SetParameter("Height", value);
        }

        public double Width
        {
            get => Parameters.GetValue<double>("Width");
            set => SetParameter("Width", value);
        }

        public double Depth
        {
            get => Parameters.GetValue<double>("Depth");
            set => SetParameter("Depth", value);
        }

        /// <summary>True for a circular section, false for rectangular.</summary>
        public bool IsRound
        {
            get => Parameters.GetValue<bool>("Round");
            set => SetParameter("Round", value);
        }

        public override string ObjectType => "Column";
        public override string IfcType => "IfcColumn";

        public Column() : base(BimDocument.GetOrCreateStyle("Default Wall"))
        {
            Name = "Column";
            Layer = "Columns";

            Parameters.Add(new BimParameter("Height", 3.0, "Column height in meters") { Unit = "m" });
            Parameters.Add(new BimParameter("Width", 0.3, "Section width in meters") { Unit = "m" });
            Parameters.Add(new BimParameter("Depth", 0.3, "Section depth in meters") { Unit = "m" });
            Parameters.Add(new BimParameter("Round", false, "Use a circular section"));
        }

        public override GeometryBase CreateGeometry()
        {
            double height = Height <= 0 ? 3.0 : Height;
            double width = Width <= 0 ? 0.3 : Width;
            double depth = Depth <= 0 ? 0.3 : Depth;
            var basePlane = new Plane(BasePoint, Vector3d.ZAxis);

            if (IsRound)
            {
                var circle = new Circle(basePlane, width / 2.0);
                var cyl = new Cylinder(circle, height);
                return cyl.ToBrep(true, true);
            }

            var box = new Box(basePlane,
                new Interval(-width / 2.0, width / 2.0),
                new Interval(-depth / 2.0, depth / 2.0),
                new Interval(0, height));
            return box.ToBrep();
        }

        public override void ExportToIfc(IfcExporter exporter)
        {
            int productId = exporter.AddRecord(
                $"IFCCOLUMN('{Id:N}',$,'{Name}',$,$,$,$,$,.COLUMN.)");
            exporter.AddPropertySet(productId, this);
        }

        public override void ImportFromIfc(object ifcEntity) { }

        public override BimObject Clone()
        {
            var clone = new Column { Name = Name, Layer = Layer, Style = Style, BasePoint = BasePoint };
            clone.Parameters.Clear();
            foreach (var p in Parameters) clone.Parameters.Add(p.Clone());
            return clone;
        }
    }
}
