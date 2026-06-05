# Samples

Place example documents here:

- `Rhino/` — `.3dm` files demonstrating the `VA_*` commands.
- `Grasshopper/` — `.gh` definitions wiring up the VisualARQ-Mac components.

Binary sample files are intentionally not committed in this proof-of-concept
scaffold. To create your own:

1. **Rhino sample** — draw a closed rectangle, run `VA_Wall` on each edge, add a
   `VA_Door` and `VA_Window`, then save as `Rhino/example.3dm`.
2. **Grasshopper sample** — drop a *Curve* param, feed it into **Create Wall**,
   and bake the result. Save as `Grasshopper/example.gh`.

See [`../docs/USAGE.md`](../docs/USAGE.md) for step-by-step instructions.
