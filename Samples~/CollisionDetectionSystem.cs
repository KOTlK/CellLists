using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Transform = CellListsECS.Runtime.Components.Transform;

namespace CellListsECS.Samples
{
    public class CollisionDetectionSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _collisionsWorld = "Collisions";
        private readonly EcsFilterInject<Inc<Cell, CellNeighbours>> _cellsFilter = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<AABB> _aabbs = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;
        private readonly EcsPoolInject<Collision> _collisions = "Collisions";

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _cellsFilter.Value)
            {
                ref var neighbours = ref _neighbours.Value.Get(entity);
                var entities = new NativeList<(Transform, AABB, int)>(Allocator.TempJob);
                var output = new NativeQueue<Collision>(Allocator.TempJob);

                
                foreach (var transformEntity in neighbours.ContainingTransforms)
                {
                    ref var transform = ref _transforms.Value.Get(transformEntity);
                    ref var aabb = ref _aabbs.Value.Get(transformEntity);
                    entities.Add((transform, aabb, transformEntity));
                }

                foreach (var neighbourEntity in neighbours.NeighboursEntities)
                {
                    ref var neighbour = ref _neighbours.Value.Get(neighbourEntity);

                    foreach (var transformEntity in neighbour.ContainingTransforms)
                    {
                        ref var transform = ref _transforms.Value.Get(transformEntity);
                        ref var aabb = ref _aabbs.Value.Get(transformEntity);
                        entities.Add((transform, aabb, transformEntity));
                    }
                }

                var job = new CollisionDetectionJob()
                {
                    Entities = entities,
                    OccuredCollisions = output.AsParallelWriter()
                };

                job.Schedule(entities.Length, 32).Complete();
                

                while (output.Count > 0)
                {
                    var collision = output.Dequeue();
                    var collisionEntity = _collisionsWorld.Value.NewEntity();
                    ref var collisionComponent = ref _collisions.Value.Add(collisionEntity);
                    collisionComponent = collision;
                }

                entities.Dispose();
                output.Dispose();
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