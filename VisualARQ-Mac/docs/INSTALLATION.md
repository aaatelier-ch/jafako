# Installation

## Prerequisites

| Requirement | Version |
|-------------|---------|
| Rhino for macOS | 7.0 or later |
| .NET SDK | 6.0+ (7.0 recommended) |
| IDE | VS Code + C# Dev Kit (recommended) |

## Option A — Install a pre-built plugin

1. Obtain or build `VisualARQMac.rhp` (see [`../build/README.md`](../build/README.md)).
2. Copy it into Rhino's Mac plugin folder:
   ```bash
   cp VisualARQMac.rhp \
     ~/Library/Application\ Support/McNeel/Rhinoceros/MacPlugIns/
   ```
3. Restart Rhino 7.
4. Confirm it loaded: run `PlugInManager` and look for **VisualARQ-Mac**, or
   simply type `VA_Wall` at the command line.

## Option B — Build from source

```bash
git clone <repo-url>
cd VisualARQ-Mac
dotnet build src/VisualARQMac/VisualARQMac.csproj -c Release
```

The build produces `VisualARQMac.rhp` in
`src/VisualARQMac/bin/Release/net7.0/`. Copy it to the plugin folder as in
Option A.

> **RhinoCommon resolution**: The project references the `RhinoCommon` and
> `Grasshopper` NuGet packages. If you have Rhino installed locally you may
> instead reference the assemblies directly from the Rhino app bundle
> (`/Applications/Rhino 7.app/Contents/Frameworks/RhCore.framework/...`).

## Verifying the install

After restarting Rhino, the command line should print:

```
VisualARQ-Mac plugin loaded successfully!
VisualARQ-Mac BIM Document initialized.
VisualARQ-Mac Grasshopper components registered.
```

## Uninstalling

Remove `VisualARQMac.rhp` from the `MacPlugIns` folder and restart Rhino.
