using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeekerGrid : MonoBehaviour
{
    private Node[,] _gridArray_2D;

    private SeekerManager.SeekerType _seekerType;

    private float _nodeDiameter;

    private int _gridSizeX, _gridSizeY;

    private Terrain _terrain;

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

    public void SetTerrain(Terrain terrain)
    {
        if (terrain == null)
        {
            return;
        }

        _terrain = terrain;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridInitialize(Vector3 startPos, Vector3 targetPos)
    {
        Vector2 dist;

        dist.x = Mathf.Abs(startPos.x - targetPos.x) * 2;
        dist.y = Mathf.Abs(startPos.z - targetPos.z) * 2;

        Vector2 gridWorldSize = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).gridWorldSize;

        if (gridWorldSize.x < dist.x)
        {
            gridWorldSize.x = dist.x;
        }

        if (gridWorldSize.y < dist.y)
        {
            gridWorldSize.y = dist.y;
        }

        _nodeDiameter = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        CreateSeekerGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridInitialize()
    {
        Vector2 gridWorldSize = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).gridWorldSize;

        _nodeDiameter = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        CreateSeekerGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    void CreateSeekerGrid()
    {
        Vector2 gridWorldSize = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).gridWorldSize;
        float nodeRadius = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).nodeRadius;

        _gridArray_2D = new Node[_gridSizeX, _gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + SeekerManager.Instance.GetSeekerGridDatas(_seekerType).nodeRadius) + Vector3.forward * (y * _nodeDiameter + nodeRadius);
                worldPoint.y = GetTerrainPositionY(worldPoint);

                _gridArray_2D[x, y] = new Node(worldPoint, x, y);
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

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_gridArray_2D[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private float GetTerrainPositionY(Vector3 worldPoint)
    {
        if (_terrain == null)
        {
            return worldPoint.y;
        }

        float terrainWorldY = _terrain.transform.position.y;
        float terrainSampleLocal = _terrain.SampleHeight(worldPoint);
        float terrainSampleWorld = terrainWorldY + terrainSampleLocal;
        return terrainSampleWorld;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector2 gridWorldSize = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).gridWorldSize;

        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        return _gridArray_2D[x, y];
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridExpand()
    {
        Vector2 gridWorldSize = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).gridWorldSize;
        Vector2 worldSizeExpandStepMax = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).worldSizeExpandStepMax;
        Vector2 worldSizeExpandStep = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).worldSizeExpandStep;

        if (worldSizeExpandStepMax == gridWorldSize)
        {
            Debug.Log("MAX boyutlara ulaştı daha fazla arama yapmaz, rame yazık zavallı ram, bulamadık abi yol yok otur ağla, bilmem belki de dünya düzdür ve biz de sonuna gelmişizdir ondan da olabilir");
            return;
        }

        if (worldSizeExpandStepMax.x >= gridWorldSize.x + worldSizeExpandStep.x)
        {
            gridWorldSize.x = worldSizeExpandStepMax.x;
        }
        else
        {
            gridWorldSize.x += worldSizeExpandStep.x;
        }

        if (worldSizeExpandStepMax.y >= gridWorldSize.y + worldSizeExpandStep.y)
        {
            gridWorldSize.y = worldSizeExpandStepMax.y;
        }
        else
        {
            gridWorldSize.y += worldSizeExpandStep.y;
        }

        SeekerGridInitialize();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private bool IsWalkableControl_TEST_FOR_GIZMOS(Vector3 worldPoint)
    {
        Collider[] hitcollider = Physics.OverlapSphere(worldPoint, SeekerManager.Instance.GetSeekerGridDatas(_seekerType).nodeRadius);

        if (hitcollider == null)
        {
            return true;
        }

        if (hitcollider.Length == 0)
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
    void OnDrawGizmos()
    {
        if (pathTEST == null)
        {
            return;
        }

        if (pathTEST.Count == 0)
        {
            return;
        }

        Vector2 gridWorldSize = SeekerManager.Instance.GetSeekerGridDatas(_seekerType).gridWorldSize;

        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (_gridArray_2D != null)
        {
            foreach (Node n in _gridArray_2D)
            {
                Gizmos.color = IsWalkableControl_TEST_FOR_GIZMOS(n.worldPosition) ? Color.white : Color.red;
                if (pathTEST != null)
                    if (pathTEST.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (_nodeDiameter - .1f));
            }
        }
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

    //     if (pathTEST == null)
    //     {
    //         return;
    //     }

    //     if (grid != null)
    //     {
    //         foreach (Node n in grid)
    //         {
    //             if (pathTEST.Contains(n))
    //             {
    //                 Gizmos.color = Color.black;
    //                 Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
    //             }
    //         }
    //     }
    // }
}