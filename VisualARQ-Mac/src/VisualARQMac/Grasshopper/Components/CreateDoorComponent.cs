using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using VisualARQMac.Objects;

namespace VisualARQMac.Grasshopper.Components
{
    /// <summary>Grasshopper component that creates a parametric Door.</summary>
    public class CreateDoorComponent : GH_Component
    {
        public CreateDoorComponent()
            : base("Create Door", "Door",
                   "Create a parametric BIM door at a point.",
                   GhCategory.Main, GhCategory.Create)
        {
        }

        public override Guid ComponentGuid => new Guid("D4E5F6A7-B8C9-0123-DEF4-567890123456");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Insertion point", GH_ParamAccess.item, Point3d.Origin);
            pManager.AddNumberParameter("Width", "W", "Door width (m)", GH_ParamAccess.item, 0.9);
            pManager.AddNumberParameter("Height", "H", "Door height (m)", GH_ParamAccess.item, 2.1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Door", "D", "Generated door geometry", GH_ParamAccess.item);
            pManager.AddGenericParameter("Id", "Id", "BIM object id", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            Point3d insert = Point3d.Origin;
            double width = 0.9, height = 2.1;

            da.GetData(0, ref insert);
            da.GetData(1, ref width);
            da.GetData(2, ref height);

            var door = new Door { BasePlane = new Plane(insert, Vector3d.ZAxis) };
            door.Parameters["Width"].Value = width;
            door.Parameters["Height"].Value = height;

            da.SetData(0, door.CreateGeometry() as Brep);
            da.SetData(1, door.Id);
        }
    }
}
