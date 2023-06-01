using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Transform = CellListsECS.Runtime.Components.Transform;

namespace CellListsECS.Samples
{
    public class DisplayCollisionsSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Collision>> _filter = "Collisions";
        private readonly EcsPoolInject<Collision> _collisions = "Collisions";
        private readonly EcsPoolInject<Transform> _transforms = default;

        public void Run(IEcsSystems systems)
        {
            
            foreach (var entity in _filter.Value)
            {
                ref var collision = ref _collisions.Value.Get(entity);
                ref var senderTransform = ref _transforms.Value.Get(collision.Sender);
                ref var receiverTransform = ref _transforms.Value.Get(collision.Receiver);

                Debug.DrawLine(senderTransform.Position, receiverTransform.Position, Color.green);
            }
        }
    }
}