using System.Collections.Generic;

namespace CellListsECS.Runtime.Components
{
    public struct CellNeighbours
    {
        public int[] NeighboursEntities;
        public List<int> ContainingTransforms;
    }
}