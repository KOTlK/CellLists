using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace CellListsECS.Runtime.Systems
{
    public class InsertTransformSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<InsertInCellLists, Transform>> _filter = default;
        private readonly EcsFilterInject<Inc<Cell, CellNeighbours>> _cellLists = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<InsertInCellLists> _commands = default;
        private readonly EcsPoolInject<Cell> _cells = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var transform = ref _transforms.Value.Get(entity);

                foreach (var cellEntity in _cellLists.Value)
                {
                    ref var cell = ref _cells.Value.Get(cellEntity);

                    if (cell.ContainsPoint(transform.Position))
                    {
                        ref var neighbour = ref _neighbours.Value.Get(cellEntity);
                        neighbour.ContainingTransforms.Add(entity);
                        transform.Cell = cellEntity;
                    }
                }

                _commands.Value.Del(entity);
            }
        }
    }
}