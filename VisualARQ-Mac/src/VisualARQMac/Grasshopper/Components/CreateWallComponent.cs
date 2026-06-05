using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using VisualARQMac.Objects;

namespace VisualARQMac.Grasshopper.Components
{
    /// <summary>Grasshopper component that creates a parametric Wall.</summary>
    public class CreateWallComponent : GH_Component
    {
        public CreateWallComponent()
            : base("Create Wall", "Wall",
                   "Create a parametric BIM wall from a base curve.",
                   GhCategory.Main, GhCategory.Create)
        {
        }

        public override Guid ComponentGuid => new Guid("C3D4E5F6-A7B8-9012-CDEF-345678901234");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Path", "P", "Wall base curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Wall height (m)", GH_ParamAccess.item, 3.0);
            pManager.AddNumberParameter("Thickness", "T", "Wall thickness (m)", GH_ParamAccess.item, 0.2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Wall", "W", "Generated wall geometry", GH_ParamAccess.item);
            pManager.AddGenericParameter("Id", "Id", "BIM object id", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            Curve path = null;
            double height = 3.0, thickness = 0.2;

            if (!da.GetData(0, ref path) || path == null) return;
            da.GetData(1, ref height);
            da.GetData(2, ref thickness);

            var wall = new Wall { Path = path.DuplicateCurve() };
            wall.Parameters["Height"].Value = height;
            wall.Parameters["Thickness"].Value = thickness;

            var geometry = wall.CreateGeometry() as Brep;
            da.SetData(0, geometry);
            da.SetData(1, wall.Id);
        }
    }
}
