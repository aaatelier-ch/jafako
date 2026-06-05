using Rhino.Geometry;
using VisualARQMac.Core;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Objects
{
    /// <summary>Represents a parametric window object (frame + glazing).</summary>
    public class Window : BimObject
    {
        /// <summary>Insertion plane; X is the window width direction, Z is up.</summary>
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

        public double SillHeight
        {
            get => Parameters.GetValue<double>("SillHeight");
            set => SetParameter("SillHeight", value);
        }

        public override string ObjectType => "Window";
        public override string IfcType => "IfcWindow";

        public Window() : base(BimDocument.GetOrCreateStyle("Default Window"))
        {
            Name = "Window";
            Layer = "Windows";

            Parameters.Add(new BimParameter("Width", 1.2, "Window width in meters") { Unit = "m", IfcProperty = "OverallWidth" });
            Parameters.Add(new BimParameter("Height", 1.2, "Window height in meters") { Unit = "m", IfcProperty = "OverallHeight" });
            Parameters.Add(new BimParameter("SillHeight", 0.9, "Sill height above floor in meters") { Unit = "m" });
        }

        public override GeometryBase CreateGeometry()
        {
            double w = Width <= 0 ? 1.2 : Width;
            double h = Height <= 0 ? 1.2 : Height;
            double sill = SillHeight < 0 ? 0.9 : SillHeight;

            var box = new Box(BasePlane,
                new Interval(-w / 2.0, w / 2.0),
                new Interval(-0.05, 0.05),
                new Interval(sill, sill + h));
            return box.ToBrep();
        }

        public override void ExportToIfc(IfcExporter exporter)
        {
            int productId = exporter.AddRecord(
                $"IFCWINDOW('{Id:N}',$,'{Name}',$,$,$,$,$,{Height.ToString(System.Globalization.CultureInfo.InvariantCulture)},{Width.ToString(System.Globalization.CultureInfo.InvariantCulture)},.NOTDEFINED.,.NOTDEFINED.,$)");
            exporter.AddPropertySet(productId, this);
        }

        public override void ImportFromIfc(object ifcEntity) { }

        public override BimObject Clone()
        {
            var clone = new Window { Name = Name, Layer = Layer, Style = Style, BasePlane = BasePlane };
            clone.Parameters.Clear();
            foreach (var p in Parameters) clone.Parameters.Add(p.Clone());
            return clone;
        }
    }
}
