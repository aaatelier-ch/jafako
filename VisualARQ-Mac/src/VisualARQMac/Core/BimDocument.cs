using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.DocObjects;

namespace VisualARQMac.Core
{
    /// <summary>Manages the BIM document and all BIM objects in the session.</summary>
    public static class BimDocument
    {
        private static readonly Dictionary<Guid, BimObject> _objects = new Dictionary<Guid, BimObject>();
        private static readonly Dictionary<string, BimStyle> _styles = new Dictionary<string, BimStyle>();
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            LoadDefaultStyles();

            RhinoDoc.AddRhinoObject += OnRhinoObjectAdded;
            RhinoDoc.DeleteRhinoObject += OnRhinoObjectDeleted;

            RhinoApp.WriteLine("VisualARQ-Mac BIM Document initialized.");
        }

        public static void Shutdown()
        {
            if (!_initialized) return;

            RhinoDoc.AddRhinoObject -= OnRhinoObjectAdded;
            RhinoDoc.DeleteRhinoObject -= OnRhinoObjectDeleted;

            _objects.Clear();
            _styles.Clear();
            _initialized = false;

            RhinoApp.WriteLine("VisualARQ-Mac BIM Document shutdown.");
        }

        public static IEnumerable<BimObject> GetAllObjects() => _objects.Values.ToList();

        public static IEnumerable<BimObject> GetObjectsOfType(string objectType) =>
            _objects.Values.Where(o => string.Equals(o.ObjectType, objectType, StringComparison.OrdinalIgnoreCase)).ToList();

        public static bool TryGetObject(Guid id, out BimObject bimObject) =>
            _objects.TryGetValue(id, out bimObject);

        public static void RegisterObject(BimObject bimObject)
        {
            if (bimObject == null) return;
            _objects[bimObject.Id] = bimObject;
        }

        public static void UnregisterObject(Guid id) => _objects.Remove(id);

        public static BimStyle GetOrCreateStyle(string name, BimStyle defaultStyle = null)
        {
            if (_styles.TryGetValue(name, out var style))
                return style;

            style = defaultStyle ?? new BimStyle(name);
            _styles[name] = style;
            return style;
        }

        public static IEnumerable<BimStyle> GetAllStyles() => _styles.Values.ToList();

        private static void LoadDefaultStyles()
        {
            var wallStyle = new BimStyle("Default Wall")
            {
                Color = System.Drawing.Color.FromArgb(200, 200, 200),
                LineWeight = 2,
                MaterialName = "Concrete"
            };
            wallStyle.Parameters.Add(new BimParameter("Thickness", 0.2, "Wall thickness in meters") { Unit = "m" });
            _styles["Default Wall"] = wallStyle;

            var doorStyle = new BimStyle("Default Door")
            {
                Color = System.Drawing.Color.FromArgb(150, 100, 50),
                LineWeight = 1,
                MaterialName = "Wood"
            };
            doorStyle.Parameters.Add(new BimParameter("Width", 0.9, "Door width in meters") { Unit = "m" });
            doorStyle.Parameters.Add(new BimParameter("Height", 2.1, "Door height in meters") { Unit = "m" });
            _styles["Default Door"] = doorStyle;

            var windowStyle = new BimStyle("Default Window")
            {
                Color = System.Drawing.Color.FromArgb(100, 150, 200),
                LineWeight = 1,
                MaterialName = "Glass"
            };
            windowStyle.Parameters.Add(new BimParameter("Width", 1.2, "Window width in meters") { Unit = "m" });
            windowStyle.Parameters.Add(new BimParameter("Height", 1.2, "Window height in meters") { Unit = "m" });
            _styles["Default Window"] = windowStyle;
        }

        private static void OnRhinoObjectAdded(object sender, RhinoObjectEventArgs e)
        {
            // BIM objects tag their backing Rhino object with a user string. When
            // such an object is (re)added we simply make sure it is tracked.
            var id = e.TheObject.Attributes.GetUserString(BimObject.UserStringKey);
            if (string.IsNullOrEmpty(id)) return;
            // Already-registered objects need no action here; this hook exists so
            // future undo/redo synchronisation can be added in one place.
        }

        private static void OnRhinoObjectDeleted(object sender, RhinoObjectEventArgs e)
        {
            var id = e.TheObject.Attributes.GetUserString(BimObject.UserStringKey);
            if (Guid.TryParse(id, out var bimId))
                UnregisterObject(bimId);
        }
    }
}
