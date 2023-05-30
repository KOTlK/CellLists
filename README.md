# CellLists
Algorithm that divides world into small same pieces for cheaper collision detection etc.

# Dependencies
[LeoEcsLite](https://github.com/Leopotam/ecslite)

[LeoEcsLite-di](https://github.com/Leopotam/ecslite-di)

Unity dependencies - inside Package.json
# Installation
## Git

Make sure you have standalone git installed.

- Install dependencies listed above.
- Unity -> PackageManager -> Add package from git URL
- Paste: ``` https://github.com/KOTlK/CellLists.git ```

## As Unity Package

- Download latest package from release page
- Assets -> Import Package -> Custom Package...
- Select downloaded package in explorer window


# Usage

Simple example of usage with scene can be found inside `Samples~` folder

- Add `CellListsInitSystem` into your systems
- Add `InsertTransformSystem` into your systems
- Add `CellListsRebuildSystem` into your systems
- Create entity with `CreateCellLists` component attached to it
- Fill component values
  
Sequence above will create cells in the next `CellListsInitSystem.Execute` call.

To insert `Transform` into cells simply add component `InsertInCellLists` to transform.

To get cells, filter components with `Cell, CellNeighbours, TransformContainer` components.

`Cell` is a cell. `CellNeighbours` contains indexes of `Cell`s, closest to the cell. TransformContainer contains indexes of `Transform` components, that belongs to the cell

`CellListsRebuildSystem` can be called with some interval via [Interval Systems](https://github.com/nenuacho/ecslite-interval-systems) extension.

You can Replace `Transform` component with any component, that have `public UnityEngine.Vector2 Position` field in it by cloning repository and deleting my transform from `Components` folder or by downloading unity package and doing the same. Just make sure your component is unmanaged, so it could be used in a parallel jobs