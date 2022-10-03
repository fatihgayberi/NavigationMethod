using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    #region "Grid for Vaue"
    /*****************************************************/
    [Header("===============Grid for Vaue===============")]
    /*****************************************************/
    [SerializeField] private Terrain terrain;
    [SerializeField] private Vector2 worldSizeExpandStep;
    [SerializeField] private Vector2 worldSizeExpandStepMax;
    #endregion
    /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

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

    public void GridInitialize(Vector3 startPos, Vector3 targetPos)
    {
        Vector2 dist;

        dist.x = Mathf.Abs(startPos.x - targetPos.x) * 2;
        dist.y = Mathf.Abs(startPos.z - targetPos.z) * 2;

        if (gridWorldSize.x < dist.x)
        {
            gridWorldSize.x = dist.x;
        }

        if (gridWorldSize.y < dist.y)
        {
            gridWorldSize.y = dist.y;
        }

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void GridInitialize()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                worldPoint.y = GetTerrainPositionY(worldPoint);

                grid[x, y] = new Node(worldPoint, x, y);
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

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private float GetTerrainPositionY(Vector3 worldPoint)
    {
        if (terrain == null)
        {
            return worldPoint.y;
        }

        float terrainWorldY = terrain.transform.position.y;
        float terrainSampleLocal = terrain.SampleHeight(worldPoint);
        float terrainSampleWorld = terrainWorldY + terrainSampleLocal;
        return terrainSampleWorld;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void GridExpand()
    {
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

        GridInitialize();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private bool IsWalkableControl_TEST_FOR_GIZMOS(Vector3 worldPoint)
    {
        Collider[] hitcollider = Physics.OverlapSphere(worldPoint, nodeRadius);

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

        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = IsWalkableControl_TEST_FOR_GIZMOS(n.worldPosition) ? Color.white : Color.red;
                if (pathTEST != null)
                    if (pathTEST.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
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