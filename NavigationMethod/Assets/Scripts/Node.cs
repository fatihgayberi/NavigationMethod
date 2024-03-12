using UnityEngine;
using System.Collections;

namespace HHG.PathfindingSystem
{
    public struct Node : IHeapItem<Node>
    {
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;

        private int gCost;
        private int hCost;
        private int parentX;
        private int parentY;
        private int heapIndex;

        public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) : this()
        {
            walkable = _walkable;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
        }

        public int fCost
        {
            get
            {
                return GCost + HCost;
            }
        }

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public int GCost { get => gCost; set => gCost = value; }
        public int HCost { get => hCost; set => hCost = value; }
        public int ParentX { get => parentX; set => parentX = value; }
        public int ParentY { get => parentY; set => parentY = value; }

        public void SetGCost(int newGCost) { gCost = newGCost; }
        public void SetHCost(int newHCost) { hCost = newHCost; }

        public void SetParentX(int newParentX) { parentX = newParentX; }
        public void SetParentY(int newParentY) { parentY = newParentY; }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(nodeToCompare.HCost);
            }
            return -compare;
        }
    }
}