using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeekerGrid : MonoBehaviour
{
    /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    #region "SeekerGrid for Vaue"
    /*****************************************************/
    [Header("===============SeekerGrid for Vaue===============")]
    /*****************************************************/
    [SerializeField] private Terrain terrain;
    [SerializeField] private Vector2 worldSizeExpandStep;
    [SerializeField] private Vector2 worldSizeExpandStepMax;
    #endregion
    /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    public Vector2 seekerGridWorldSize;
    public float nodeRadius;
    Node[,] seekerGrid;

    float nodeDiameter;
    int seekerGridSizeX, seekerGridSizeY;

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

    public void SeekerGridInitialize(Vector3 startPos, Vector3 targetPos)
    {
        Vector2 dist;

        dist.x = Mathf.Abs(startPos.x - targetPos.x) * 2;
        dist.y = Mathf.Abs(startPos.z - targetPos.z) * 2;

        if (seekerGridWorldSize.x < dist.x)
        {
            seekerGridWorldSize.x = dist.x;
        }

        if (seekerGridWorldSize.y < dist.y)
        {
            seekerGridWorldSize.y = dist.y;
        }

        nodeDiameter = nodeRadius * 2;
        seekerGridSizeX = Mathf.RoundToInt(seekerGridWorldSize.x / nodeDiameter);
        seekerGridSizeY = Mathf.RoundToInt(seekerGridWorldSize.y / nodeDiameter);
        CreateSeekerGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridInitialize()
    {
        nodeDiameter = nodeRadius * 2;
        seekerGridSizeX = Mathf.RoundToInt(seekerGridWorldSize.x / nodeDiameter);
        seekerGridSizeY = Mathf.RoundToInt(seekerGridWorldSize.y / nodeDiameter);
        CreateSeekerGrid();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    void CreateSeekerGrid()
    {
        seekerGrid = new Node[seekerGridSizeX, seekerGridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * seekerGridWorldSize.x / 2 - Vector3.forward * seekerGridWorldSize.y / 2;

        for (int x = 0; x < seekerGridSizeX; x++)
        {
            for (int y = 0; y < seekerGridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                worldPoint.y = GetTerrainPositionY(worldPoint);

                seekerGrid[x, y] = new Node(worldPoint, x, y);
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

                if (checkX >= 0 && checkX < seekerGridSizeX && checkY >= 0 && checkY < seekerGridSizeY)
                {
                    neighbours.Add(seekerGrid[checkX, checkY]);
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
        float percentX = (worldPosition.x + seekerGridWorldSize.x / 2) / seekerGridWorldSize.x;
        float percentY = (worldPosition.z + seekerGridWorldSize.y / 2) / seekerGridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((seekerGridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((seekerGridSizeY - 1) * percentY);
        return seekerGrid[x, y];
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerGridExpand()
    {
        if (worldSizeExpandStepMax == seekerGridWorldSize)
        {
            Debug.Log("MAX boyutlara ulaştı daha fazla arama yapmaz, rame yazık zavallı ram, bulamadık abi yol yok otur ağla, bilmem belki de dünya düzdür ve biz de sonuna gelmişizdir ondan da olabilir");
            return;
        }

        if (worldSizeExpandStepMax.x >= seekerGridWorldSize.x + worldSizeExpandStep.x)
        {
            seekerGridWorldSize.x = worldSizeExpandStepMax.x;
        }
        else
        {
            seekerGridWorldSize.x += worldSizeExpandStep.x;
        }

        if (worldSizeExpandStepMax.y >= seekerGridWorldSize.y + worldSizeExpandStep.y)
        {
            seekerGridWorldSize.y = worldSizeExpandStepMax.y;
        }
        else
        {
            seekerGridWorldSize.y += worldSizeExpandStep.y;
        }

        SeekerGridInitialize();
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

        Gizmos.DrawWireCube(transform.position, new Vector3(seekerGridWorldSize.x, 1, seekerGridWorldSize.y));

        if (seekerGrid != null)
        {
            foreach (Node n in seekerGrid)
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
    //     Gizmos.DrawWireCube(transform.position, new Vector3(seekerGridWorldSize.x, 1, seekerGridWorldSize.y));

    //     if (pathTEST == null)
    //     {
    //         return;
    //     }

    //     if (seekerGrid != null)
    //     {
    //         foreach (Node n in seekerGrid)
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