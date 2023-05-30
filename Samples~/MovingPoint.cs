using UnityEngine;

namespace CellListsECS.Samples
{
    public struct MovingPoint
    {
        public Vector2 RandomDirection;
        public float TimeSinceLastDirectionChange;
    }
}