using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;

namespace CellListsECS.Samples
{
    public class CollisionDetectionSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _collisionsWorld = "Collisions";
        private readonly EcsFilterInject<Inc<Cell>> _cellsFilter = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<AABB> _aabbs = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;
        private readonly EcsPoolInject<TransformContainer> _containers = default;
        private readonly EcsPoolInject<Collision> _collisions = "Collisions";

        private static readonly ProfilerMarker ListBuild = new(nameof(ListBuild));
        private static readonly ProfilerMarker Job = new(nameof(Job));
        private static readonly ProfilerMarker CollisionsWriting = new(nameof(CollisionsWriting));
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _cellsFilter.Value)
            {
                ListBuild.Begin();
                ref var neighbours = ref _neighbours.Value.Get(entity);
                ref var cellContainer = ref _containers.Value.Get(entity);
                var entities = new NativeList<(Transform, AABB, int)>(Allocator.TempJob);
                var output = new NativeQueue<Collision>(Allocator.TempJob);

                foreach (var transformEntity in cellContainer.All)
                {
                    ref var transform = ref _transforms.Value.Get(transformEntity);
                    ref var aabb = ref _aabbs.Value.Get(transformEntity);
                    entities.Add((transform, aabb, transformEntity));
                }

                foreach (var neighbour in neighbours.All)
                {
                    ref var neighbourContainer = ref _containers.Value.Get(neighbour);

                    foreach (var transformEntity in neighbourContainer.All)
                    {
                        ref var transform = ref _transforms.Value.Get(transformEntity);
                        ref var aabb = ref _aabbs.Value.Get(transformEntity);
                        entities.Add((transform, aabb, transformEntity));
                    }
                }
                ListBuild.End();

                Job.Begin();
                var job = new CollisionDetectionJob()
                {
                    Entities = entities,
                    OccuredCollisions = output.AsParallelWriter()
                };

                job.Schedule(entities.Length, 32).Complete();
                
                Job.End();

                CollisionsWriting.Begin();
                while (output.Count > 0)
                {
                    var collision = output.Dequeue();
                    var collisionEntity = _collisionsWorld.Value.NewEntity();
                    ref var collisionComponent = ref _collisions.Value.Add(collisionEntity);
                    collisionComponent = collision;
                }

                entities.Dispose();
                output.Dispose();
                CollisionsWriting.End();
            }
        }
        
        [BurstCompile]
        public struct CollisionDetectionJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<(Transform, AABB, int)> Entities;
            public NativeQueue<Collision>.ParallelWriter OccuredCollisions;
            
            [BurstCompile]
            public void Execute(int index)
            {
                var (transform, aabb, entity) = Entities[index];

                foreach (var (secondTransform, secondAABB, secondEntity) in Entities)
                {
                    if(entity == secondEntity)
                        continue;

                    if (CollisionDetected.AABBAgainstAABB(transform, aabb, secondTransform, secondAABB))
                    {
                        OccuredCollisions.Enqueue(new Collision()
                        {
                            Sender = entity,
                            Receiver = secondEntity
                        });
                    }
                }
            }
        }
    }
}