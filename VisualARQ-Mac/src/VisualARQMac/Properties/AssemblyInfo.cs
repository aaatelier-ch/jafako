using System.Reflection;
using System.Runtime.InteropServices;
using Rhino.PlugIns;

// Plugin description shown in Rhino's PlugInManager.
[assembly: PlugInDescription(DescriptionType.Address, "")]
[assembly: PlugInDescription(DescriptionType.Country, "International")]
[assembly: PlugInDescription(DescriptionType.Email, "community@visualarq-mac.org")]
[assembly: PlugInDescription(DescriptionType.Phone, "")]
[assembly: PlugInDescription(DescriptionType.Fax, "")]
[assembly: PlugInDescription(DescriptionType.Organization, "VisualARQ-Mac Open Source Community")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, "https://github.com/visualarq-mac/visualarq-mac")]
[assembly: PlugInDescription(DescriptionType.WebSite, "https://github.com/visualarq-mac/visualarq-mac")]

[assembly: AssemblyTitle("VisualARQMac")]
[assembly: AssemblyDescription("Open-source BIM plugin for Rhino 7 macOS")]
[assembly: AssemblyCompany("VisualARQ-Mac Open Source Community")]
[assembly: AssemblyProduct("VisualARQ-Mac")]
[assembly: AssemblyCopyright("Copyright © 2026")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// The plugin's unique id. Must match VisualARQMacPlugin.PluginGuid.
[assembly: Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890")]
