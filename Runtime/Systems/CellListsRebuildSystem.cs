using CellListsECS.Runtime.Components;
using CellListsECS.Runtime.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace CellListsECS.Runtime.Systems
{
    public class CellListsRebuildSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Cell, CellNeighbours, TransformContainer>> _cellsFilter = default;
        private readonly EcsPoolInject<Cell> _cells = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;
        private readonly EcsPoolInject<TransformContainer> _containers = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _cellsFilter.Value)
            {
                ref var cell = ref _cells.Value.Get(entity);
                ref var cellNeighbours = ref _neighbours.Value.Get(entity);
                ref var transformContainer = ref _containers.Value.Get(entity);
                var neighbours = new NativeArray<(Cell, int)>(cellNeighbours.All.Length, Allocator.TempJob);
                var transforms = new NativeArray<(Transform, int)>(transformContainer.All.Count, Allocator.TempJob);
                var removeCommands = new NativeQueue<int>(Allocator.TempJob);
                var addCommands = new NativeQueue<(int, int)>(Allocator.TempJob);

                for (var i = 0; i < cellNeighbours.All.Length; i++)
                {
                    var neighbourEntity = cellNeighbours.All[i];
                    ref var component = ref _cells.Value.Get(neighbourEntity);
                    neighbours[i] = (component, neighbourEntity);
                }

                for (var i = 0; i < transformContainer.All.Count; i++)
                {
                    var transformEntity = transformContainer.All[i];
                    ref var transform = ref _transforms.Value.Get(transformEntity);
                    transforms[i] = (transform, transformEntity);
                }

                var job = new RebuildJob()
                {
                    Cell = cell,
                    Neighbours = neighbours,
                    Transforms = transforms,
                    RemoveCommands = removeCommands.AsParallelWriter(),
                    AddCommands = addCommands.AsParallelWriter()
                }.Schedule(transforms.Length, 32);
                
                job.Complete();

                while (removeCommands.Count > 0)
                {
                    var i = removeCommands.Dequeue();
                    transformContainer.All.Remove(i);
                }

                while (addCommands.Count > 0)
                {
                    var (cellEntity, transformEntity) = addCommands.Dequeue();

                    ref var targetContainer = ref _containers.Value.Get(cellEntity);
                    targetContainer.All.Add(transformEntity);
                }

                neighbours.Dispose();
                transforms.Dispose();
                addCommands.Dispose();
                removeCommands.Dispose();
            }
        }
        
        [BurstCompile]
        public struct RebuildJob : IJobParallelFor
        {
            [ReadOnly] public Cell Cell;
            [ReadOnly] public NativeArray<(Cell, int)> Neighbours;
            [ReadOnly] public NativeArray<(Transform, int)> Transforms;

            /// <summary>
            /// transformEntity
            /// </summary>
            public NativeQueue<int>.ParallelWriter RemoveCommands;

            /// <summary>
            /// cellEntity, transformEntity;
            /// </summary>
            public NativeQueue<(int, int)>.ParallelWriter AddCommands;

            [BurstCompile]
            public void Execute(int index)
            {
                var (transform, transformEntity) = Transforms[index];

                if (CollisionDetection.AABBContainsPoint(Cell.Position, Cell.AABB, transform.Position))
                    return;
                
                RemoveCommands.Enqueue(transformEntity);

                foreach (var (neighbourCell, neighbourEntity) in Neighbours)
                {
                    if (CollisionDetection.AABBContainsPoint(neighbourCell.Position, neighbourCell.AABB, transform.Position))
                    {
                        AddCommands.Enqueue((neighbourEntity, transformEntity));
                        break;
                    }
                }
            }
        }
    }
}