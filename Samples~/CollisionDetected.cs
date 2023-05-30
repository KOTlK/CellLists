using CellListsECS.Runtime.Components;

namespace CellListsECS.Samples
{
    public static class CollisionDetected
    {
        public static bool AABBAgainstAABB(Transform transform, AABB aabb, Transform secondTransform, AABB secondAABB)
        {
            return transform.Position.x < secondTransform.Position.x + secondAABB.Size.x &&
                   transform.Position.x + aabb.Size.x > secondTransform.Position.x &&
                   transform.Position.y < secondTransform.Position.y + secondAABB.Size.y &&
                   transform.Position.y + secondAABB.Size.y > secondTransform.Position.y;
        }
    }
}