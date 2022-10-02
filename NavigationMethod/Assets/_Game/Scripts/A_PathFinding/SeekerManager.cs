using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wonnasmith;

[Serializable]
public class SeekerManager : Singleton<SeekerManager>
{
    public enum SeekerType
    {
        NONE,

        Seeker_1
    }

    [Serializable]
    class SeekerDatas
    {
        public SeekerType seekerType;
        public GridDatas gridDatas;
        public PathfindingDatas pathfindingDatas;
    }

    [Serializable]
    public class GridDatas
    {
        [Tooltip("Gorus mesafemi kacar adim genisleteyim")]
        public Vector2 worldSizeExpandStep;

        [Tooltip("Max ne kadarlık bir alan icerisnde arama yapayim")]
        public Vector2 worldSizeExpandStepMax;

        [Tooltip("Default gorus mesafem kac olsun")]
        public Vector2 gridWorldSize;

        [Tooltip("Araziyi ne kadar detayli arayalim")]
        public float nodeRadius;
    }

    [Serializable]
    public class PathfindingDatas
    {
        [Tooltip("Max ne kadar uzaksam yeni yol arayayim")]
        public float distTolerance;

        [Tooltip("Yeni rota hesaplamak icin kac saniye bekleyelim")]
        public float newRotaWaitTime;

        [Tooltip("Egim max yuzde kac olursa yuruyebileyim")]
        [Range(0f, 100f)]
        public float maxSeekerSlope;
    }

    [Tooltip("Ne kadar geniş bir alan icerisinde terrain arasın")]
    [SerializeField]
    private float terrainFindRadius;

    [SerializeField] private SeekerDatas[] seekerDatasArray;

    public delegate void SeekerManagerSeekerCharacterMovePathGenerator(List<Pathfinding> pathfindingList, Vector3 targetPos);

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void OnEnable()
    {
        SeekerInputMoveController.SeekerCharacterMovePathGenerator += OnSeekerCharacterMovePathGenerator;
    }

    private void OnDisable()
    {
        SeekerInputMoveController.SeekerCharacterMovePathGenerator -= OnSeekerCharacterMovePathGenerator;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void OnSeekerCharacterMovePathGenerator(List<Pathfinding> pathfindingList, Vector3 targetPos)
    {
        if (pathfindingList == null)
        {
            return;
        }

        int pathfindingListCount = pathfindingList.Count;

        for (int i = 0; i < pathfindingListCount; i++)
        {
            pathfindingList[i].FindPath(targetPos);
        }
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public float GetTerrainFindRadius()
    {
        return terrainFindRadius;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public GridDatas GetSeekerGridDatas(SeekerType seekerType)
    {
        int seekerDatasArrayLength = seekerDatasArray.Length;

        for (int i = 0; i < seekerDatasArrayLength; i++)
        {
            if (seekerDatasArray[i].seekerType == seekerType)
            {
                return seekerDatasArray[i].gridDatas;
            }
        }

        return null;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public PathfindingDatas GetSeekerPathfindingDatas(SeekerType seekerType)
    {
        int seekerDatasArrayLength = seekerDatasArray.Length;

        for (int i = 0; i < seekerDatasArrayLength; i++)
        {
            if (seekerDatasArray[i].seekerType == seekerType)
            {
                return seekerDatasArray[i].pathfindingDatas;
            }
        }

        return null;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]
}
