using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Transform = CellListsECS.Runtime.Components.Transform;

namespace CellListsECS.Samples
{
    public class DisplayNeighboursSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Transform>> _filter = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<CellNeighbours> _cellNeighbours = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var transform = ref _transforms.Value.Get(entity);
                
                if(transform.Cell == int.MinValue)
                    continue;
                
                ref var neighbours = ref _cellNeighbours.Value.Get(transform.Cell);

                foreach (var neighbourTransformEntity in neighbours.ContainingTransforms)
                {
                    ref var neighbourTransform = ref _transforms.Value.Get(neighbourTransformEntity);

                    Debug.DrawLine(transform.Position, neighbourTransform.Position, Color.yellow);
                }

                foreach (var neighbourEntity in neighbours.NeighboursEntities)
                {
                    ref var neighbour = ref _cellNeighbours.Value.Get(neighbourEntity);

                    foreach (var neighbourTransformEntity in neighbour.ContainingTransforms)
                    {
                        ref var neighbourTransform = ref _transforms.Value.Get(neighbourTransformEntity);
                        
                        Debug.DrawLine(transform.Position, neighbourTransform.Position, Color.yellow);
                    }
                }
            }
        }
    }
}