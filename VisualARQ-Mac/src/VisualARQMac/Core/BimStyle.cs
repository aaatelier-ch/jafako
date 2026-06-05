using System.Drawing;

namespace VisualARQMac.Core
{
    /// <summary>Represents a reusable style for BIM objects.</summary>
    public class BimStyle
    {
        public string Name { get; set; }
        public Color Color { get; set; } = Color.Black;
        public int LineWeight { get; set; } = 1;
        public string MaterialName { get; set; } = "Default";
        public DisplayMode DisplayMode { get; set; } = DisplayMode.Shaded;

        /// <summary>Default parameters applied to objects created with this style.</summary>
        public BimParameterCollection Parameters { get; } = new BimParameterCollection();

        public BimStyle(string name)
        {
            Name = name;
        }

        public BimStyle Clone()
        {
            var clone = new BimStyle(Name + " (Copy)")
            {
                Color = Color,
                LineWeight = LineWeight,
                MaterialName = MaterialName,
                DisplayMode = DisplayMode
            };

            foreach (var param in Parameters)
                clone.Parameters.Add(param.Clone());

            return clone;
        }
    }

    /// <summary>Display mode for a <see cref="BimStyle"/>.</summary>
    public enum DisplayMode
    {
        Wireframe,
        Shaded,
        Rendered
    }
}
