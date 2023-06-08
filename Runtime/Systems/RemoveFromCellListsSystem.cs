using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace CellListsECS.Runtime.Systems
{
    public class RemoveFromCellListsSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _eventsWorld = default;
        private readonly EcsFilterInject<Inc<CellNeighbours>> _cellsFilter = default;
        private readonly EcsFilterInject<Inc<RemoveFromCellLists>> _filter = default;
        private readonly EcsPoolInject<RemoveFromCellLists> _commands = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var command = ref _commands.Value.Get(entity);

                foreach (var neighbourEntity in _cellsFilter.Value)
                {
                    ref var neighbour = ref _neighbours.Value.Get(neighbourEntity);

                    if (neighbour.ContainingTransforms.Contains(command.TransformEntity))
                    {
                        neighbour.ContainingTransforms.Remove(command.TransformEntity);
                        break;
                    }
                }

                _eventsWorld.Value.DelEntity(entity);
            }
        }
    }
}