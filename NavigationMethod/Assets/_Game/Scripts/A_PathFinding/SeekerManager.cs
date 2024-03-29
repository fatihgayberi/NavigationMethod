using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wonnasmith;

[Serializable]
public class SeekerManager : Singleton<SeekerManager>
{
    public delegate void SeekerManagerSeekerCharacterMovePathGenerator(List<SeekerPathfinding> seekerPathfindingList, Vector3 targetPos);

    [Serializable]
    class SeekerDatas
    {
        [Tooltip("SeekerLabel")]
        public string strSeekerLabel;

        [Tooltip("Ne kadar detaylı arama yapsın (node büyüklüğü)")]
        public float nodeRadius;

        public GridDatas gridDatas;

        public SeekerPathfindingDatas seekerPathfindingDatas;
    }

    [Serializable]
    public class GridDatas
    {
        [Tooltip("Gorus mesafemi kacar adim genisleteyim")]
        public Vector2 worldSizeExpandStep;

        [Tooltip("Max ne kadarlık bir alan icerisnde arama yapayim")]
        public Vector2 worldSizeExpandStepMax;

        // [Tooltip("Default gorus mesafem kac olsun")]
        // public Vector2 gridWorldSize;
    }

    [Serializable]
    public class SeekerPathfindingDatas
    {
        [Tooltip("Max ne kadar uzaksam yeni yol arayayim")]
        public float distTolerance;

        [Tooltip("Yeni rota hesaplamak icin kac saniye bekleyelim")]
        public float newRotaWaitTime;

        [Tooltip("Egim max yuzde kac olursa yuruyebileyim")]
        [Range(0f, 100f)]
        public float maxSeekerSlope;
    }

    [SerializeField] private SeekerDatas[] seekerDatasArray;

    private void OnEnable()
    {
        SeekerInputMoveController.SeekerCharacterMovePathGenerator += OnSeekerCharacterMovePathGenerator;
    }
    private void OnDisable()
    {
        SeekerInputMoveController.SeekerCharacterMovePathGenerator -= OnSeekerCharacterMovePathGenerator;
    }

    private void OnSeekerCharacterMovePathGenerator(List<SeekerPathfinding> seekerPathfindingList, Vector3 targetPos)
    {
        if (seekerPathfindingList == null)
        {
            return;
        }

        int seekerPathfindingListCount = seekerPathfindingList.Count;

        for (int i = 0; i < seekerPathfindingListCount; i++)
        {
            seekerPathfindingList[i].FindPath(targetPos);
        }
    }

    private SeekerDatas GetSeekerDatas(int idx)
    {
        if (seekerDatasArray == null)
        {
            return new SeekerDatas();
        }

        int seekerDatasArrayLength = seekerDatasArray.Length;

        if (idx > seekerDatasArrayLength)
        {
            return new SeekerDatas();
        }

        if (idx < 0)
        {
            return new SeekerDatas();
        }

        return seekerDatasArray[idx];
    }

    public int GetSeekerLabelIdx(string strSeekerLabel)
    {
        int seekerDatasArrayLength = seekerDatasArray.Length;

        for (int i = 0; i < seekerDatasArrayLength; i++)
        {
            if (seekerDatasArray[i].strSeekerLabel == strSeekerLabel)
            {
                return i;
            }
        }

        return -1;
    }

    public float GetNodeRadius(int seekderDataIdx)
    {
        SeekerDatas seekerDatas = GetSeekerDatas(seekderDataIdx);

        if (seekerDatas == null)
        {
            return 0.5f;
        }

        return seekerDatas.nodeRadius;
    }

    public GridDatas GetSeekerGridDatas(int seekderDataIdx)
    {
        SeekerDatas seekerDatas = GetSeekerDatas(seekderDataIdx);

        if (seekerDatas == null)
        {
            seekerDatas = new SeekerDatas();
        }

        if (seekerDatas.gridDatas == null)
        {
            seekerDatas.gridDatas = new GridDatas();
        }

        return seekerDatas.gridDatas;
    }

    public SeekerPathfindingDatas GetSeekerSeekerPathfindingDatas(int seekderDataIdx)
    {
        SeekerDatas seekerDatas = GetSeekerDatas(seekderDataIdx);

        if (seekerDatas == null)
        {
            seekerDatas = new SeekerDatas();
        }

        if (seekerDatas.seekerPathfindingDatas == null)
        {
            seekerDatas.seekerPathfindingDatas = new SeekerPathfindingDatas();
        }

        return seekerDatas.seekerPathfindingDatas;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]
}