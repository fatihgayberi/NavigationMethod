using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour
{
    public bool isTEST;

    /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    #region "PathFind for Value"
    /***********************************************************/
    [Header("===============PathFind for Value===============")]
    /***********************************************************/
    [Space(10)]
    [SerializeField] private Grid grid;
    [SerializeField] private float distTolerance;
    [SerializeField] private float newRotaWaitTime;
    [SerializeField] private float maxSeekerSlope;
    #endregion
    /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

    private Coroutine _newRotaCoroutine;
    private WaitForSeconds _newRotaCalculateWaitTime;
    private Vector3 _targetPos;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void FindPath(Vector3 targetPos)
    {
        _targetPos = targetPos;

        grid.GridInitialize(Vector3.zero, _targetPos - transform.position);

        Node startNode = grid.NodeFromWorldPoint(Vector3.zero);
        Node targetNode = grid.NodeFromWorldPoint(_targetPos - transform.position);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                    {
                        node = openSet[i];
                    }
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            else if (openSet.Count == 1)
            {
                grid.GridExpand();
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!IsWalkableControl(neighbour.worldPosition) || closedSet.Contains(neighbour) || !IsVerySlopeControl(neighbour, node))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SeekerDetecetNavObs()
    {
        float dist = Vector3.Distance(_targetPos, transform.position);

        if (dist <= distTolerance)
        {
            Debug.Log("DUR YOLCU 07");

            // return;
        }

        Debug.Log("Yeni rota hesaplanıyor");

        if (_newRotaCoroutine != null)
        {
            StopCoroutine(_newRotaCoroutine);
        }

        _newRotaCoroutine = StartCoroutine(NewRotaGeneratorIEnumerator());
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private IEnumerator NewRotaGeneratorIEnumerator()
    {
        if (_newRotaCalculateWaitTime == null)
        {
            _newRotaCalculateWaitTime = new WaitForSeconds(newRotaWaitTime);
        }

        yield return _newRotaCalculateWaitTime;

        FindPath(_targetPos);
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private bool IsWalkableControl(Vector3 worldPoint)
    {
        Collider[] hitcollider = Physics.OverlapSphere(worldPoint, grid.nodeRadius);

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

    private bool IsVerySlopeControl(Node neighbour, Node node)
    {
        if (isTEST)
        {
            return true;
        }

        float slope = SlopeCalculator(neighbour, node);

        if (slope <= maxSeekerSlope)
        {
            return true;
        }

        return false;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private float SlopeCalculator(Node neighbour, Node node)
    {
        if (neighbour == null)
        {
            return 0;
        }

        if (node == null)
        {
            return 0;
        }

        Vector3 neighbourPos = neighbour.worldPosition;
        Vector3 nodePos = node.worldPosition;

        float delta_Y = Mathf.Abs(neighbourPos.y - nodePos.y);
        float dist = Vector3.Distance(neighbourPos, nodePos);

        float slope = delta_Y / dist;

        Debug.Log("delta_Y:::" + delta_Y);
        Debug.Log("dist:::" + dist);
        Debug.Log("slope:::" + (delta_Y / dist));
        Debug.Log("slope_Yuzde:::" + Wonnasmith.FloatExtensionMethods.FloatRemap(slope, 0, 1, 0, 100));

        return Wonnasmith.FloatExtensionMethods.FloatRemap(slope, 0, 1, 0, 100);
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.pathTEST = path;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
