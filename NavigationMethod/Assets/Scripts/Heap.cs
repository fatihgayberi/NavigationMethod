using System;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace HHG.PathfindingSystem
{
    [Serializable]
    public struct Heap<T> where T : struct, IHeapItem<T>
    {
        private T[] items;
        private int currentItemCount;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
            currentItemCount = 0;
        }

        public void Add(ref T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(ref item);
            currentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount -= 1;

            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;

            T refItem = items[0];

            SortDown(ref refItem);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(ref item);
        }

        public int Count => currentItemCount;

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        private void SortDown(ref T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;

                if (childIndexLeft >= currentItemCount) return;

                int swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) >= 0) return;

                T swapItem = items[swapIndex];

                Swap(ref item, ref swapItem);
            }
        }

        private void SortUp(ref T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];

                if (item.CompareTo(parentItem) <= 0) break;

                Swap(ref item, ref parentItem);

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void Swap(ref T itemA, ref T itemB)
        {
            T itemTempA = itemA;
            T itemTempB = itemB;

            items[itemA.HeapIndex] = itemTempB;
            items[itemB.HeapIndex] = itemTempA;

            items[itemTempA.HeapIndex].HeapIndex = itemTempB.HeapIndex;
            items[itemTempB.HeapIndex].HeapIndex = itemTempA.HeapIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
