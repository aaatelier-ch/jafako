# VisualARQ-Mac: Open-Source BIM Plugin for Rhino 7 macOS

> **Status**: Proof-of-Concept Scaffold | **Version**: 1.0.0 | **Rhino Version**: 7.0+

A complete, open-source BIM (Building Information Modeling) plugin scaffold for
Rhino 7 on macOS, providing VisualARQ-like functionality with parametric
architectural objects, IFC support, and Grasshopper integration.

> ⚠️ **Note**: This is a proof-of-concept scaffold. It demonstrates the full
> architecture, file layout, and API surface of a BIM plugin. The RhinoCommon /
> Grasshopper API calls are written to be representative; you will need the
> Rhino 7 SDK installed to build, and some method bodies are intentionally
> simplified placeholders marked with `// TODO`.

---

## 📦 Project Structure

```
VisualARQ-Mac/
├── src/
│   ├── VisualARQMac/
│   │   ├── Core/
│   │   │   ├── BimObject.cs          # Base BIM object class
│   │   │   ├── BimDocument.cs        # Document management
│   │   │   ├── BimStyle.cs           # Object styling system
│   │   │   ├── BimParameter.cs       # Parameter system
│   │   │   └── Ifc/
│   │   │       ├── IfcExporter.cs    # IFC export functionality
│   │   │       └── IfcImporter.cs    # IFC import functionality
│   │   ├── Objects/
│   │   │   ├── Wall.cs               # Wall object implementation
│   │   │   ├── Door.cs               # Door object implementation
│   │   │   ├── Window.cs             # Window object implementation
│   │   │   ├── Column.cs             # Column object implementation
│   │   │   ├── Slab.cs               # Slab object implementation
│   │   │   ├── Stair.cs              # Stair object implementation
│   │   │   └── Roof.cs               # Roof object implementation
│   │   ├── Commands/
│   │   │   ├── CreateWallCommand.cs
│   │   │   ├── CreateDoorCommand.cs
│   │   │   ├── CreateWindowCommand.cs
│   │   │   ├── CreateColumnCommand.cs
│   │   │   ├── CreateSlabCommand.cs
│   │   │   ├── CreateStairCommand.cs
│   │   │   ├── CreateRoofCommand.cs
│   │   │   ├── ExportIfcCommand.cs
│   │   │   ├── ImportIfcCommand.cs
│   │   │   └── BimManagerCommand.cs
│   │   ├── Grasshopper/
│   │   │   ├── VisualARQMacGrasshopperPlugin.cs
│   │   │   └── Components/
│   │   │       ├── CreateWallComponent.cs
│   │   │       ├── CreateDoorComponent.cs
│   │   │       └── GetBimObjectsComponent.cs
│   │   ├── Properties/
│   │   │   └── AssemblyInfo.cs
│   │   ├── VisualARQMacPlugin.cs     # Main plugin class
│   │   ├── VisualARQMac.info         # Plugin metadata
│   │   └── VisualARQMac.csproj       # Project file
│   └── VisualARQMac.sln              # Solution
├── build/
│   └── README.md                     # How the .rhp package is produced
├── docs/
│   ├── INSTALLATION.md
│   ├── USAGE.md
│   └── API.md
├── samples/
│   ├── Grasshopper/
│   └── Rhino/
├── LICENSE
├── README.md
└── .gitignore
```

---

## 🚀 Quick Start

### Prerequisites
- **Rhino 7 for macOS** (7.0 or later)
- **.NET 6.0+ SDK** (for building)
- **Visual Studio Code** (recommended IDE)
- **C# Dev Kit** extension for VS Code

### Installation (Pre-built Plugin)
1. Build `VisualARQMac.rhp` (see `build/README.md`)
2. Copy it to: `~/Library/Application Support/McNeel/Rhinoceros/MacPlugIns/`
3. Restart Rhino 7
4. The plugin will load automatically

### Building from Source
```bash
cd VisualARQ-Mac
dotnet build src/VisualARQMac/VisualARQMac.csproj -c Release
# Output: src/VisualARQMac/bin/Release/net7.0/VisualARQMac.rhp
```

See [`docs/INSTALLATION.md`](docs/INSTALLATION.md) for the full guide.

---

## ⚙️ Architecture Overview

| Layer | Location | Responsibility |
|-------|----------|----------------|
| **Plugin** | `VisualARQMacPlugin.cs` | Lifecycle, command discovery, GH registration |
| **Document** | `Core/BimDocument.cs` | Tracks all BIM objects & styles, Rhino event sync |
| **Objects** | `Core/BimObject.cs`, `Objects/*` | Parametric architectural elements |
| **Styles** | `Core/BimStyle.cs` | Display + default parameter presets |
| **Parameters** | `Core/BimParameter.cs` | Typed BIM data with IFC mapping |
| **IFC** | `Core/Ifc/*` | STEP/IFC import & export |
| **Commands** | `Commands/*` | `VA_*` Rhino commands |
| **Grasshopper** | `Grasshopper/*` | Parametric component library |

See [`docs/API.md`](docs/API.md) for the full API reference and
[`docs/USAGE.md`](docs/USAGE.md) for command-by-command usage.

---

## 🧱 Object Types

| Command | Object | IFC Type |
|---------|--------|----------|
| `VA_Wall` | `Wall` | `IfcWall` |
| `VA_Door` | `Door` | `IfcDoor` |
| `VA_Window` | `Window` | `IfcWindow` |
| `VA_Column` | `Column` | `IfcColumn` |
| `VA_Slab` | `Slab` | `IfcSlab` |
| `VA_Stair` | `Stair` | `IfcStair` |
| `VA_Roof` | `Roof` | `IfcRoof` |

---

## 📄 License

MIT — see [LICENSE](LICENSE). VisualARQ® is a trademark of Asuni CAD, S.A.
This project is an independent, open-source alternative and is not affiliated
with or endorsed by Asuni.
