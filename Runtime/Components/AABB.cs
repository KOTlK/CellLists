using System;
using UnityEngine;

namespace CellListsECS.Runtime.Components
{
    [Serializable]
    public struct AABB
    {
        public Vector2 Size;
        public Vector2 HalfExtents => Size * 0.5f;
    }
}