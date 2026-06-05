using System;
using System.Linq;
using Grasshopper.Kernel;
using VisualARQMac.Core;

namespace VisualARQMac.Grasshopper.Components
{
    /// <summary>Grasshopper component that queries existing BIM objects.</summary>
    public class GetBimObjectsComponent : GH_Component
    {
        public GetBimObjectsComponent()
            : base("Get BIM Objects", "GetBIM",
                   "Query the BIM objects in the current document, optionally filtered by type.",
                   GhCategory.Main, GhCategory.Query)
        {
        }

        public override Guid ComponentGuid => new Guid("E5F6A7B8-C9D0-1234-EF56-789012345678");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Type", "T", "Object type filter (e.g. Wall). Leave empty for all.",
                GH_ParamAccess.item, string.Empty);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Ids", "Id", "BIM object ids", GH_ParamAccess.list);
            pManager.AddTextParameter("Names", "N", "BIM object names", GH_ParamAccess.list);
            pManager.AddTextParameter("Types", "T", "BIM object types", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            string typeFilter = string.Empty;
            da.GetData(0, ref typeFilter);

            var objects = string.IsNullOrWhiteSpace(typeFilter)
                ? BimDocument.GetAllObjects()
                : BimDocument.GetObjectsOfType(typeFilter);

            var list = objects.ToList();
            da.SetDataList(0, list.Select(o => o.Id));
            da.SetDataList(1, list.Select(o => o.Name));
            da.SetDataList(2, list.Select(o => o.ObjectType));
        }
    }
}
