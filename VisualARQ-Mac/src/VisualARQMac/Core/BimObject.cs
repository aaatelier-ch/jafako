using System;
using System.Collections.Generic;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using VisualARQMac.Core.Ifc;

namespace VisualARQMac.Core
{
    /// <summary>Base class for all BIM objects.</summary>
    public abstract class BimObject : IDisposable
    {
        /// <summary>User-string key used to tag the backing Rhino object.</summary>
        public const string UserStringKey = "VisualARQ-Mac";

        /// <summary>Unique identifier for the BIM object.</summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>Id of the backing Rhino object in the active document.</summary>
        public Guid RhinoObjectId { get; private set; } = Guid.Empty;

        /// <summary>Object name.</summary>
        public string Name { get; set; } = "BIM Object";

        /// <summary>Object type (Wall, Door, Window, ...).</summary>
        public abstract string ObjectType { get; }

        /// <summary>Reference to the style.</summary>
        public BimStyle Style { get; set; }

        /// <summary>Collection of parameters.</summary>
        public BimParameterCollection Parameters { get; } = new BimParameterCollection();

        /// <summary>Parent object (for nested objects).</summary>
        public BimObject Parent { get; set; }

        /// <summary>Child objects.</summary>
        public List<BimObject> Children { get; } = new List<BimObject>();

        /// <summary>Layer name.</summary>
        public string Layer { get; set; } = "Default";

        /// <summary>IFC type mapping.</summary>
        public virtual string IfcType => "IfcBuildingElementProxy";

        protected BimObject()
        {
            BimDocument.RegisterObject(this);
        }

        protected BimObject(BimStyle style) : this()
        {
            Style = style;
        }

        /// <summary>Resolve the backing Rhino object from the active document.</summary>
        public RhinoObject RhinoObject =>
            RhinoObjectId == Guid.Empty ? null : RhinoDoc.ActiveDoc?.Objects.FindId(RhinoObjectId);

        /// <summary>Create the Rhino geometry for this object.</summary>
        public abstract GeometryBase CreateGeometry();

        /// <summary>Create or update the backing Rhino object.</summary>
        public virtual void Update()
        {
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null) return;

            var geometry = CreateGeometry();
            if (geometry == null) return;

            int layerIndex = EnsureLayer(doc, Layer);
            var attributes = new ObjectAttributes
            {
                Name = Name,
                LayerIndex = layerIndex,
                ColorSource = ObjectColorSource.ColorFromObject,
                ObjectColor = Style?.Color ?? System.Drawing.Color.Black
            };
            attributes.SetUserString(UserStringKey, Id.ToString());

            var existing = RhinoObject;
            if (existing == null)
            {
                RhinoObjectId = doc.Objects.Add(geometry, attributes);
            }
            else
            {
                doc.Objects.Replace(RhinoObjectId, geometry);
                doc.Objects.ModifyAttributes(RhinoObjectId, attributes, quiet: true);
            }

            doc.Views.Redraw();
        }

        /// <summary>Delete the backing Rhino object.</summary>
        public virtual void Delete()
        {
            var doc = RhinoDoc.ActiveDoc;
            if (doc != null && RhinoObjectId != Guid.Empty)
            {
                doc.Objects.Delete(RhinoObjectId, quiet: true);
            }
            RhinoObjectId = Guid.Empty;
        }

        /// <summary>Get a parameter value by name.</summary>
        public object GetParameter(string name) =>
            Parameters.TryGetValue(name, out var parameter) ? parameter.Value : null;

        /// <summary>Set a parameter value by name and rebuild the geometry.</summary>
        public void SetParameter(string name, object value)
        {
            if (Parameters.TryGetValue(name, out var parameter))
                parameter.Value = value;
            else
                Parameters.Add(new BimParameter(name, value));

            Update();
        }

        /// <summary>Export this object to IFC.</summary>
        public abstract void ExportToIfc(IfcExporter exporter);

        /// <summary>Populate this object from an IFC entity.</summary>
        public abstract void ImportFromIfc(object ifcEntity);

        /// <summary>Get the world bounding box of the backing geometry.</summary>
        public virtual BoundingBox GetBoundingBox()
        {
            var ro = RhinoObject;
            return ro?.Geometry?.GetBoundingBox(false) ?? BoundingBox.Empty;
        }

        /// <summary>Create a deep copy of this object.</summary>
        public abstract BimObject Clone();

        /// <summary>Ensure a layer with the given name exists, returning its index.</summary>
        protected static int EnsureLayer(RhinoDoc doc, string name)
        {
            var existing = doc.Layers.FindName(name);
            if (existing != null) return existing.Index;
            return doc.Layers.Add(name, System.Drawing.Color.Black);
        }

        public void Dispose()
        {
            Delete();
            BimDocument.UnregisterObject(Id);
        }
    }
}
