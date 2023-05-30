using System;
using UnityEngine;

namespace CellListsECS.Runtime.Components
{
    [Serializable]
    public struct Cell
    {
        public AABB AABB;
        public Vector2 Position;
    }
}