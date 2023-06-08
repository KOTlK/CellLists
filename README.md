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

Simple example of usage can be found inside `Samples~` folder

- Add `CellListsInitSystem` into your systems
- Add `InsertTransformSystem` into your systems
- Add `RemoveFromCellListsSystem` into your systems
- Add `CellListsRebuildSystem` into your systems
- Create `CellListsConfig` and inject it in your systems

``` C#
var world = new EcsWorld();
var systems = new EcsSystems(world);
var config = new CellListsConfig()
{
    Center = Vector2.zero,
    Size = new Vector2(100, 100),
    Height = 10,
    Width = 10
};

systems
    .Add(new CellListsInitSystem())
    .Add(new InsertTransformSystem())
    .Add(new RemoveFromCellListsSystem())
    .Add(new CellListsRebuildSystem())
    .Inject(config)
    .Init();
```
  

To insert `Transform` into cells add component `InsertInCellLists` to transform.

To remove `Transform` component from cell, create new entity with `RemoveFromCellLists` component on it and fill field `TransformEntity`

To get cells, filter components with `Cell, CellNeighbours` components.

`Cell` is a cell. `CellNeighbours` contains indexes of `Cell`s, closest to the cell, and indexes of `Transform` components, that belongs to the cell. Transform components contains its position and cell, it belongs to.

`CellListsRebuildSystem` can be called with some interval via [Interval Systems](https://github.com/nenuacho/ecslite-interval-systems) extension.