using CellListsECS.Runtime.Components;
using UnityEngine;

namespace CellListsECS.Runtime.Utils
{
    public static class CollisionDetection
    {
        public static bool AABBContainsPoint(Vector2 position, AABB aabb, Vector2 point)
        {
            var halfExtents = aabb.HalfExtents;
            return position.x + halfExtents.x >= point.x && 
                   position.y + halfExtents.y >= point.y &&
                   position.x - halfExtents.x <= point.x &&
                   position.y - halfExtents.y <= point.y;
        }
    }
}