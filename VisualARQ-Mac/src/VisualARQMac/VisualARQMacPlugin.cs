using System;
using Rhino;
using Rhino.PlugIns;
using VisualARQMac.Core;
using VisualARQMac.Grasshopper;

namespace VisualARQMac
{
    /// <summary>
    /// Main plugin class for VisualARQ-Mac.
    /// </summary>
    /// <remarks>
    /// Rhino discovers and registers every <see cref="Rhino.Commands.Command"/>
    /// subclass in this assembly automatically, so there is no manual command
    /// registration step. The plugin only needs to wire up the BIM document and
    /// the Grasshopper component library.
    /// </remarks>
    public sealed class VisualARQMacPlugin : PlugIn
    {
        public static VisualARQMacPlugin Instance { get; private set; }

        public VisualARQMacPlugin()
        {
            Instance = this;
        }

        /// <summary>Plugin GUID - must be unique and match the .sln project GUID.</summary>
        public override Guid PluginGuid => new Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");

        /// <summary>Called when the plugin is loaded.</summary>
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            try
            {
                // Initialise the BIM document layer.
                BimDocument.Initialize();

                // Register Grasshopper components.
                GrasshopperPlugin.Register();

                RhinoApp.WriteLine("VisualARQ-Mac plugin loaded successfully!");
                return LoadReturnCode.Success;
            }
            catch (Exception ex)
            {
                errorMessage = $"VisualARQ-Mac failed to load: {ex.Message}";
                return LoadReturnCode.ErrorShowDialog;
            }
        }

        /// <summary>Called when the plugin is unloaded.</summary>
        protected override void OnShutdown()
        {
            GrasshopperPlugin.Unregister();
            BimDocument.Shutdown();
            Instance = null;

            RhinoApp.WriteLine("VisualARQ-Mac plugin unloaded.");
        }
    }
}
