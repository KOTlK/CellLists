using System;
using UnityEngine;

namespace CellListsECS.Runtime.Components
{
    [Serializable]
    public struct CreateCellLists
    {
        public Vector2 Size;
        public Vector2 Center;
        public int Width;
        public int Height;
    }
}