using System;
using UnityEngine;

namespace CellListsECS.Runtime.Components
{
    [Serializable]
    public struct Cell
    {
        public AABB AABB;
        public Vector2 Position;

        public bool ContainsPoint(Vector2 point)
        {
            var halfExtents = AABB.HalfExtents;
            return Position.x + halfExtents.x >= point.x && 
                   Position.y + halfExtents.y >= point.y &&
                   Position.x - halfExtents.x <= point.x &&
                   Position.y - halfExtents.y <= point.y;
        }
    }
}