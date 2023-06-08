using System;
using UnityEngine;

namespace CellListsECS.Runtime.Components
{
    [Serializable]
    public struct Transform
    {
        public Vector2 Position;
        public int Cell;
    }
}