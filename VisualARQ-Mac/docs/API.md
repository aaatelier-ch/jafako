# API Reference

Namespaces: `VisualARQMac`, `VisualARQMac.Core`, `VisualARQMac.Core.Ifc`,
`VisualARQMac.Objects`, `VisualARQMac.Commands`, `VisualARQMac.Grasshopper`.

## Core

### `BimObject` (abstract)
Base class for every architectural element.

| Member | Description |
|--------|-------------|
| `Guid Id` | Stable unique id. |
| `Guid RhinoObjectId` | Id of the backing Rhino object (`Guid.Empty` if none). |
| `RhinoObject RhinoObject` | Resolves the backing object from the active doc. |
| `string ObjectType` | e.g. `"Wall"` (abstract). |
| `string IfcType` | e.g. `"IfcWall"`. |
| `BimStyle Style` | Display/parameter preset. |
| `BimParameterCollection Parameters` | Typed BIM data. |
| `GeometryBase CreateGeometry()` | Build the geometry (abstract). |
| `void Update()` | Create or refresh the backing Rhino object. |
| `void Delete()` | Remove the backing Rhino object. |
| `void SetParameter(name, value)` | Set a parameter and rebuild. |
| `void ExportToIfc(IfcExporter)` | Emit IFC records (abstract). |
| `BimObject Clone()` | Deep copy (abstract). |

### `BimDocument` (static)
Tracks all objects/styles for the session and syncs with Rhino delete events.

| Member | Description |
|--------|-------------|
| `Initialize()` / `Shutdown()` | Lifecycle, called by the plugin. |
| `GetAllObjects()` | All tracked BIM objects. |
| `GetObjectsOfType(type)` | Filter by `ObjectType`. |
| `TryGetObject(id, out obj)` | Lookup by id. |
| `RegisterObject(obj)` / `UnregisterObject(id)` | Bookkeeping. |
| `GetOrCreateStyle(name, default)` | Style registry. |

### `BimStyle`
`Name`, `Color`, `LineWeight`, `MaterialName`, `DisplayMode`, `Parameters`,
`Clone()`.

### `BimParameter` / `BimParameterCollection`
Typed key/value with `Type`, `Unit`, `IfcProperty`, `IsTypeParameter`.
`GetValue<T>()` performs conversion. The collection is name-indexed
(case-insensitive) with `TryGetValue`, `AddOrUpdate`, `GetValue<T>(name)`.

## IFC

### `IfcExporter`
`Export(IEnumerable<BimObject>, filePath)` writes an IFC4 STEP file.
`AddRecord(entity)` and `AddPropertySet(productId, obj)` are called from each
object's `ExportToIfc`.

### `IfcImporter`
`Import(filePath)` parses the DATA section and instantiates matching
`BimObject`s.

## Objects

| Class | Key properties |
|-------|----------------|
| `Wall` | `Path`, `Height`, `Thickness` |
| `Door` | `BasePlane`, `Width`, `Height`, `Thickness` |
| `Window` | `BasePlane`, `Width`, `Height`, `SillHeight` |
| `Column` | `BasePoint`, `Height`, `Width`, `Depth`, `IsRound` |
| `Slab` | `Boundary`, `Elevation`, `Thickness` |
| `Stair` | `BasePoint`, `Direction`, `TotalHeight`, `Width`, `StepCount`, `Tread` |
| `Roof` | `Boundary`, `Elevation`, `SlopeDegrees`, `Thickness` |

## Example (C#)

```csharp
using VisualARQMac.Objects;
using Rhino.Geometry;

var path = new LineCurve(new Point3d(0, 0, 0), new Point3d(5, 0, 0));
var wall = new Wall { Path = path };
wall.Height = 2.8;       // triggers a rebuild
wall.Thickness = 0.15;
wall.Update();           // adds it to the active Rhino document
```
