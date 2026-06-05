using Rhino.Geometry;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Objects
{
    /// <summary>Represents a parametric door object (frame + leaf).</summary>
    public class Door : BimObject
    {
        /// <summary>Insertion plane; X is the door width direction, Z is up.</summary>
        public Plane BasePlane { get; set; } = Plane.WorldXY;

        public double Width
        {
            get => Parameters.GetValue<double>("Width");
            set => SetParameter("Width", value);
        }

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

        public override string ObjectType => "Door";
        public override string IfcType => "IfcDoor";

        public Door() : base(BimDocument.GetOrCreateStyle("Default Door"))
        {
            Name = "Door";
            Layer = "Doors";

            Parameters.Add(new BimParameter("Width", 0.9, "Door width in meters") { Unit = "m", IfcProperty = "OverallWidth" });
            Parameters.Add(new BimParameter("Height", 2.1, "Door height in meters") { Unit = "m", IfcProperty = "OverallHeight" });
            Parameters.Add(new BimParameter("Thickness", 0.05, "Leaf thickness in meters") { Unit = "m" });
        }

        public override GeometryBase CreateGeometry()
        {
            double w = Width <= 0 ? 0.9 : Width;
            double h = Height <= 0 ? 2.1 : Height;
            double t = Thickness <= 0 ? 0.05 : Thickness;

            var box = new Box(BasePlane,
                new Interval(-w / 2.0, w / 2.0),
                new Interval(-t / 2.0, t / 2.0),
                new Interval(0, h));
            return box.ToBrep();
        }

        public override void ExportToIfc(IfcExporter exporter)
        {
            int productId = exporter.AddRecord(
                $"IFCDOOR('{Id:N}',$,'{Name}',$,$,$,$,$,{Height.ToString(System.Globalization.CultureInfo.InvariantCulture)},{Width.ToString(System.Globalization.CultureInfo.InvariantCulture)},.NOTDEFINED.,.NOTDEFINED.,$)");
            exporter.AddPropertySet(productId, this);
        }

        public override void ImportFromIfc(object ifcEntity) { }

        public override BimObject Clone()
        {
            var clone = new Door { Name = Name, Layer = Layer, Style = Style, BasePlane = BasePlane };
            clone.Parameters.Clear();
            foreach (var p in Parameters) clone.Parameters.Add(p.Clone());
            return clone;
        }
    }
}
