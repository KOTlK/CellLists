using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace CellListsECS.Samples.Scenes
{
    public class CellDrawSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Cell, CellNeighbours>> _filter = default;
        private readonly EcsPoolInject<Cell> _cells = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var cell = ref _cells.Value.Get(entity);
                var position = cell.Position;
                var halfExtents = cell.AABB.HalfExtents;
                var upLeft = new Vector2(position.x - halfExtents.x, position.y + halfExtents.y);
                var upRight = new Vector2(position.x + halfExtents.x, position.y + halfExtents.y);
                var downRight = new Vector2(position.x + halfExtents.x, position.y - halfExtents.y);
                var downLeft = new Vector2(position.x - halfExtents.x, position.y - halfExtents.y);

                Debug.DrawLine(upLeft, upRight, Color.cyan);
                Debug.DrawLine(upRight, downRight, Color.cyan);
                Debug.DrawLine(downRight, downLeft, Color.cyan);
                Debug.DrawLine(downLeft, upLeft, Color.cyan);
            }
        }
    }
}