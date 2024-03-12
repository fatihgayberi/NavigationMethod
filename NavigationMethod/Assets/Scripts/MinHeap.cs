using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.PathfindingSystem
{
    public struct MinHeap
    {
        private HeapNode[] heapArray;
        private int capacity;
        public int Count;

        public MinHeap(int capacity)
        {
            this.capacity = capacity;
            heapArray = new HeapNode[capacity];
            Count = 0;
        }

        private int GetParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        private int GetLeftChildIndex(int index)
        {
            return 2 * index + 1;
        }

        private int GetRightChildIndex(int index)
        {
            return 2 * index + 2;
        }

        private void Swap(int index1, int index2)
        {
            HeapNode temp = heapArray[index1];
            heapArray[index1] = heapArray[index2];
            heapArray[index2] = temp;
        }

        private bool IsLessThan(HeapNode node1, HeapNode node2)
        {
            if (node1.GetTotalCost() < node2.GetTotalCost())
                return true;
            else if (node1.GetTotalCost() == node2.GetTotalCost())
                return node1.H < node2.H;
            else
                return false;
        }

        private void HeapifyUp(int index)
        {
            int parentIndex = GetParentIndex(index);
            while (index > 0 && IsLessThan(heapArray[index], heapArray[parentIndex]))
            {
                Swap(parentIndex, index);
                index = parentIndex;
                parentIndex = GetParentIndex(index);
            }
        }

        private void HeapifyDown(int index)
        {
            int minIndex = index;
            int leftChildIndex = GetLeftChildIndex(index);
            int rightChildIndex = GetRightChildIndex(index);

            if (leftChildIndex < Count && IsLessThan(heapArray[leftChildIndex], heapArray[minIndex]))
            {
                minIndex = leftChildIndex;
            }

            if (rightChildIndex < Count && IsLessThan(heapArray[rightChildIndex], heapArray[minIndex]))
            {
                minIndex = rightChildIndex;
            }

            if (index != minIndex)
            {
                Swap(index, minIndex);
                HeapifyDown(minIndex);
            }
        }

        public void Insert(int g, int h)
        {
            if (Count == capacity)
            {
                Console.WriteLine("Heap is full.");
                return;
            }

            heapArray[Count] = new HeapNode(g, h);
            HeapifyUp(Count);
            Count++;
        }

        public HeapNode ExtractMin()
        {
            if (Count == 0)
            {
                Console.WriteLine("Heap is empty.");
                return default(HeapNode);
            }

            HeapNode minNode = heapArray[0];
            heapArray[0] = heapArray[Count - 1];
            Count--;
            HeapifyDown(0);

            return minNode;
        }

        public void PrintHeap()
        {
            for (int i = 0; i < Count; i++)
            {
                Console.Write("(" + heapArray[i].G + "," + heapArray[i].H + ") ");
            }
            Console.WriteLine();
        }
    }

    public struct HeapNode
    {
        public int G;
        public int H;

        public HeapNode(int g, int h)
        {
            G = g;//f;
            H = h;//g;
        }

        public int GetTotalCost()
        {
            return G + H;
        }
    }
}
