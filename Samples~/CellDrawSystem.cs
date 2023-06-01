using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Transform = CellListsECS.Runtime.Components.Transform;

namespace CellListsECS.Samples
{
    public class CellDrawSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Cell, CellNeighbours>> _filter = default;
        private readonly EcsPoolInject<Cell> _cells = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var cell = ref _cells.Value.Get(entity);
                ref var neighbours = ref _neighbours.Value.Get(entity);


                foreach (var cellNeighbour in neighbours.NeighboursEntities)
                {
                    ref var neighbour = ref _cells.Value.Get(cellNeighbour);
                    Debug.DrawLine(cell.Position, neighbour.Position, Color.red);
                }

                foreach (var containingEntity in neighbours.ContainingTransforms)
                {
                    ref var transform = ref _transforms.Value.Get(containingEntity);
                    Debug.DrawLine(cell.Position, transform.Position, Color.blue);
                }
            }
        }
    }
}