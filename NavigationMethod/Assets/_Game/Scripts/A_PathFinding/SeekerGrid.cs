﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeekerGrid : MonoBehaviour
{
    private Node[,] _seekerGridArray;
    private Vector3 _seekerGridWorldSize;

    private float _nodeDiameter;
    private int _seekerGridSizeX, _seekerGridSizeZ;

    private SeekerManager.SeekerType _seekerType;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void OnEnable()
    {
        Wonnasmith.GOD_MODE_MANAGER.GizmosClearButtonClick += OnGizmosClearButtonClick;
    }

    private void OnDisable()
    {
        Wonnasmith.GOD_MODE_MANAGER.GizmosClearButtonClick -= OnGizmosClearButtonClick;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void OnGizmosClearButtonClick()
    {
        if (pathTEST == null)
        {
            return;
        }

        pathTEST.Clear();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SetSeekerType(SeekerManager.SeekerType seekerType)
    {
        _seekerType = seekerType;
    }
    
    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridInitialize(Vector3 startPos, Vector3 targetPos)
    {
        Vector3 dist;

        dist.x = Mathf.Abs(startPos.x - targetPos.x) * 2;
        dist.z = Mathf.Abs(startPos.z - targetPos.z) * 2;

        if (_seekerGridWorldSize.x < dist.x)
        {
            _seekerGridWorldSize.x = dist.x;
        }

        if (_seekerGridWorldSize.z < dist.z)
        {
            _seekerGridWorldSize.z = dist.z;
        }

        _nodeDiameter = SeekerManager.Instance.GetNodeRadius(_seekerType) * 2;
        _seekerGridSizeX = Mathf.RoundToInt(_seekerGridWorldSize.x / _nodeDiameter);
        _seekerGridSizeZ = Mathf.RoundToInt(_seekerGridWorldSize.z / _nodeDiameter);
        CreateSeekerGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridInitialize()
    {
        _nodeDiameter = SeekerManager.Instance.GetNodeRadius(_seekerType) * 2;
        _seekerGridSizeX = Mathf.RoundToInt(_seekerGridWorldSize.x / _nodeDiameter);
        _seekerGridSizeZ = Mathf.RoundToInt(_seekerGridWorldSize.z / _nodeDiameter);
        CreateSeekerGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    void CreateSeekerGrid()
    {
        _seekerGridArray = new Node[_seekerGridSizeX, _seekerGridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * _seekerGridWorldSize.x / 2 - Vector3.forward * _seekerGridWorldSize.z / 2;

        for (int x = 0; x < _seekerGridSizeX; x++)
        {
            for (int y = 0; y < _seekerGridSizeZ; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + SeekerManager.Instance.GetNodeRadius(_seekerType)) + Vector3.forward * (y * _nodeDiameter + SeekerManager.Instance.GetNodeRadius(_seekerType));
                worldPoint.y = TerrainsManager.Instance.GetTerrainSampleHeight(worldPoint);

                _seekerGridArray[x, y] = new Node(worldPoint, x, y);
            }
        }
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.seekerGridX + x;
                int checkY = node.seekerGridY + y;

                if (checkX >= 0 && checkX < _seekerGridSizeX && checkY >= 0 && checkY < _seekerGridSizeZ)
                {
                    neighbours.Add(_seekerGridArray[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + _seekerGridWorldSize.x / 2) / _seekerGridWorldSize.x;
        float percentZ = (worldPosition.z + _seekerGridWorldSize.z / 2) / _seekerGridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((_seekerGridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((_seekerGridSizeZ - 1) * percentZ);
        return _seekerGridArray[x, z];
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridExpand()
    {
        Vector3 worldSizeExpandStepMax = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).worldSizeExpandStepMax;
        Vector3 worldSizeExpandStep = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).worldSizeExpandStep;
        
        if (worldSizeExpandStepMax == _seekerGridWorldSize)
        {
            Debug.Log("<color=red>MAX boyutlara ulaştı daha fazla arama yapmaz, rame yazık zavallı ram, bulamadık abi yol yok otur ağla, bilmem belki de dünya düzdür ve biz de sonuna gelmişizdir ondan da olabilir</color>");
            return;
        }


        if (worldSizeExpandStepMax.x >= _seekerGridWorldSize.x + worldSizeExpandStep.x)
        {
            _seekerGridWorldSize.x = worldSizeExpandStepMax.x;
        }
        else
        {
            _seekerGridWorldSize.x += worldSizeExpandStep.x;
        }

        if (worldSizeExpandStepMax.z >= _seekerGridWorldSize.z + worldSizeExpandStep.z)
        {
            _seekerGridWorldSize.z = worldSizeExpandStepMax.z;
        }
        else
        {
            _seekerGridWorldSize.z += worldSizeExpandStep.z;
        }

        SeekerGridInitialize();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private bool IsWalkableControl_TEST_FOR_GIZMOS(Vector3 worldPoint)
    {
        Collider[] hitcollider = Physics.OverlapSphere(worldPoint, SeekerManager.Instance.GetNodeRadius(_seekerType));

        if (hitcollider == null)
        {
            return true;
        }

        if (hitcollider.Length == 0)
        {
            return true;
        }

        if (hitcollider[0] == null)
        {
            return true;
        }

        if (hitcollider[0].gameObject == null)
        {
            return true;
        }

        int hitcolliderLength = hitcollider.Length;

        for (int i = 0; i < hitcolliderLength; i++)
        {
            NavObs navObs = hitcollider[i].gameObject.GetComponent<NavObs>();

            if (navObs != null)
            {
                return false;
            }
        }

        return true;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public List<Node> pathTEST;
    // void OnDrawGizmos()
    // {
    //     if (pathTEST == null)
    //     {
    //         return;
    //     }

    //     if (pathTEST.Count == 0)
    //     {
    //         return;
    //     }

    //     Gizmos.DrawWireCube(transform.position, new Vector3(seekerGridDefaultWorldSize.x, 1, seekerGridDefaultWorldSize.z));

    //     if (_seekerGrid != null)
    //     {
    //         foreach (Node n in _seekerGrid)
    //         {
    //             Gizmos.color = IsWalkableControl_TEST_FOR_GIZMOS(n.worldPosition) ? Color.white : Color.red;
    //             if (pathTEST != null)
    //                 if (pathTEST.Contains(n))
    //                     Gizmos.color = Color.black;
    //             Gizmos.DrawCube(n.worldPosition, Vector3.one * (_nodeDiameter - .1f));
    //         }
    //     }
    // }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_seekerGridWorldSize.x, 1, _seekerGridWorldSize.z));

        if (pathTEST == null)
        {
            return;
        }

        if (_seekerGridArray != null)
        {
            foreach (Node n in _seekerGridArray)
            {
                if (pathTEST.Contains(n))
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (_nodeDiameter - .1f));
                }
            }
        }
    }
}