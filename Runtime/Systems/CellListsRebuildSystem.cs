using CellListsECS.Runtime.Components;
using CellListsECS.Runtime.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Profiling;
using Transform = CellListsECS.Runtime.Components.Transform;

namespace CellListsECS.Runtime.Systems
{
    //No Jobs 100000: 2.55ms overall build
    //No Jobs 300000: 24ms overall build
    //Jobs 100000: 5ms overall build
    //Jobs 300000: 30ms overall build
    //Creating arrays for jobs takes 10x more time than an actual job
    public class CellListsRebuildSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Cell, CellNeighbours>> _cellsFilter = default;
        private readonly EcsPoolInject<Cell> _cells = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;

        private static readonly ProfilerMarker Overall = new(nameof(Overall));
        private static readonly ProfilerMarker Once = new(nameof(Once));

        public void Run(IEcsSystems systems)
        {
            Overall.Begin();
            foreach (var entity in _cellsFilter.Value)
            {
                Once.Begin();
                ref var cell = ref _cells.Value.Get(entity);
                ref var neighbours = ref _neighbours.Value.Get(entity);

                for(var i = 0; i < neighbours.ContainingTransforms.Count; i++)
                {
                    var transformEntity = neighbours.ContainingTransforms[i];
                    ref var transform = ref _transforms.Value.Get(transformEntity);
                    if (CollisionDetection.AABBContainsPoint(cell.Position, cell.AABB, transform.Position))
                        continue;

                    neighbours.ContainingTransforms.Remove(transformEntity);

                    foreach (var neighbourEntity in neighbours.NeighboursEntities)
                    {
                        ref var neighbourCell = ref _cells.Value.Get(neighbourEntity);

                        if (CollisionDetection.AABBContainsPoint(neighbourCell.Position, neighbourCell.AABB,
                                transform.Position))
                        {
                            ref var neighbourContainer = ref _neighbours.Value.Get(neighbourEntity);
                            neighbourContainer.ContainingTransforms.Add(transformEntity);
                        }
                    }
                }

                Once.End();
            }
            Overall.End();
            
        }
    }
}