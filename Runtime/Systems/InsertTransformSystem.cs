using CellListsECS.Runtime.Components;
using CellListsECS.Runtime.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace CellListsECS.Runtime.Systems
{
    public class InsertTransformSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<InsertInCellLists, Transform>> _filter = default;
        private readonly EcsFilterInject<Inc<Cell, CellNeighbours, TransformContainer>> _cellLists = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<InsertInCellLists> _commands = default;
        private readonly EcsPoolInject<Cell> _cells = default;
        private readonly EcsPoolInject<TransformContainer> _containers = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var transform = ref _transforms.Value.Get(entity);

                foreach (var cellEntity in _cellLists.Value)
                {
                    ref var cell = ref _cells.Value.Get(cellEntity);
                    ref var container = ref _containers.Value.Get(cellEntity);

                    if (CollisionDetection.AABBContainsPoint(cell.Position, cell.AABB, transform.Position))
                    {
                        container.All.Add(entity);
                    }
                }

                _commands.Value.Del(entity);
            }
        }
    }
}