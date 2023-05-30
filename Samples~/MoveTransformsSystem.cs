using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Transform = CellListsECS.Runtime.Components.Transform;

namespace CellListsECS.Samples
{
    public class MoveTransformsSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Transform, MovingPoint>> _filter = default;
        private readonly EcsPoolInject<Transform> _transforms = default;
        private readonly EcsPoolInject<MovingPoint> _points = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var transform = ref _transforms.Value.Get(entity);
                ref var point = ref _points.Value.Get(entity);
                if (point.TimeSinceLastDirectionChange <= 0)
                {
                    point.TimeSinceLastDirectionChange = 10f;
                    point.RandomDirection = Random.insideUnitCircle.normalized;
                }
                else
                {
                    point.TimeSinceLastDirectionChange -= Time.deltaTime;
                }

                transform.Position += point.RandomDirection * 5f * Time.deltaTime;
            }
        }
    }
}