using CellListsECS.Runtime.Components;
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

        private static readonly ProfilerMarker Job = new(nameof(Job));

        public void Run(IEcsSystems systems)
        {
            Job.Begin();

            foreach (var entity in _cellsFilter.Value)
            {
                ref var cell = ref _cells.Value.Get(entity);
                ref var neighbours = ref _neighbours.Value.Get(entity);

                for(var i = 0; i < neighbours.ContainingTransforms.Count; i++)
                {
                    var transformEntity = neighbours.ContainingTransforms[i];
                    ref var transform = ref _transforms.Value.Get(transformEntity);
                    if (cell.ContainsPoint(transform.Position))
                        continue;

                    neighbours.ContainingTransforms.Remove(transformEntity);
                    transform.Cell = int.MinValue;

                    foreach (var neighbourEntity in neighbours.NeighboursEntities)
                    {
                        ref var neighbourCell = ref _cells.Value.Get(neighbourEntity);

                        if (neighbourCell.ContainsPoint(transform.Position))
                        {
                            ref var neighbourContainer = ref _neighbours.Value.Get(neighbourEntity);
                            neighbourContainer.ContainingTransforms.Add(transformEntity);
                            transform.Cell = neighbourEntity;
                            break;
                        }
                    }
                }
            }


            Job.End();
        }
    }
}