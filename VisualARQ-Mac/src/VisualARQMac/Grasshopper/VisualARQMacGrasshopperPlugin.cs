using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino;

namespace VisualARQMac.Grasshopper
{
    /// <summary>
    /// Static helper used by <see cref="VisualARQMacPlugin"/> to announce the
    /// Grasshopper component library lifecycle.
    /// </summary>
    /// <remarks>
    /// Grasshopper discovers components automatically from assemblies that expose
    /// a <see cref="GH_AssemblyInfo"/> (see <see cref="VisualARQMacGrasshopperInfo"/>)
    /// and <see cref="GH_Component"/> subclasses, so registration is largely a
    /// no-op here; it exists to keep the loading sequence explicit and logged.
    /// </remarks>
    public static class GrasshopperPlugin
    {
        public static void Register()
        {
            RhinoApp.WriteLine("VisualARQ-Mac Grasshopper components registered.");
        }

        public static void Unregister()
        {
            RhinoApp.WriteLine("VisualARQ-Mac Grasshopper components unregistered.");
        }
    }

    /// <summary>Grasshopper assembly metadata for the VisualARQ-Mac component library.</summary>
    public class VisualARQMacGrasshopperInfo : GH_AssemblyInfo
    {
        public override string Name => "VisualARQ-Mac";
        public override Bitmap Icon => null;
        public override string Description => "Parametric BIM components for Rhino 7 macOS.";
        public override Guid Id => new Guid("B2C3D4E5-F6A7-8901-BCDE-F23456789012");
        public override string AuthorName => "VisualARQ-Mac Open Source Community";
        public override string AuthorContact => "community@visualarq-mac.org";
    }

    /// <summary>Shared category/sub-category constants for VisualARQ-Mac components.</summary>
    internal static class GhCategory
    {
        public const string Main = "VisualARQ-Mac";
        public const string Create = "Create";
        public const string Query = "Query";
    }
}
