using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HHG.PathfindingSystem;
using System;

[Serializable]
public class HeapTest : MonoBehaviour
{
    [Serializable]
    public class AA : IHeapItem<AA>
    {
        public int v;

        int heapIndex;

        public AA(int _v)
        {
            v = _v;
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
        public int CompareTo(AA other)
        {
            return 0;
        }
    }

    //public Heap<AA> hx;

    // Update is called once per frame
    //void Start()
    //{
    //    hx.Add(new AA(0));
    //    hx.Add(new AA(1));
    //    hx.Add(new AA(2));
    //    hx.Add(new AA(3));
    //    hx.Add(new AA(4));
    //    return;
    //    for (int i = 0; i < 5; i++)
    //    {
    //        Debug.Log(hx.RemoveFirst().v);
    //    }
    //}
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        Debug.Log(hx.RemoveFirst().v);
    //    }
    //}
}
