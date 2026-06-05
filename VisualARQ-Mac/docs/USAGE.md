# Usage

All commands are prefixed with `VA_` and can be typed at the Rhino command line.

## Commands

| Command | Description | Inputs |
|---------|-------------|--------|
| `VA_Wall` | Create a parametric wall | base curve, height, thickness |
| `VA_Door` | Place a parametric door | point, width, height |
| `VA_Window` | Place a parametric window | point, width, height, sill |
| `VA_Column` | Place a parametric column | base point, height, section size |
| `VA_Slab` | Create a slab from a closed curve | boundary curve, thickness |
| `VA_Stair` | Create a straight stair | base point, direction, height, steps |
| `VA_Roof` | Create a single-slope roof | footprint curve, slope, thickness |
| `VA_ExportIFC` | Export all BIM objects to IFC | output path |
| `VA_ImportIFC` | Import BIM objects from IFC | input path |
| `VA_Manager` | Print a summary of the BIM model | — |

## Example: a simple room

1. Draw four lines forming a rectangle in the Top view.
2. Run `VA_Wall`, select an edge, accept the defaults (height 3.0, thickness
   0.2). Repeat for each edge.
3. Run `VA_Door`, click a point on one wall, accept defaults.
4. Run `VA_Window`, click a point on another wall.
5. Run `VA_ExportIFC` and provide a path such as `~/Desktop/room.ifc`.

## Grasshopper

Open Grasshopper (`Grasshopper` command) and look under the **VisualARQ-Mac**
tab:

- **Create › Create Wall** — `Path`, `Height`, `Thickness` → `Wall` brep + `Id`
- **Create › Create Door** — `Point`, `Width`, `Height` → `Door` brep + `Id`
- **Query › Get BIM Objects** — optional `Type` → `Ids`, `Names`, `Types`

## Units

All built-in parameters are expressed in **meters** (and **degrees** for
angles). Set your Rhino document units to meters for 1:1 results.
