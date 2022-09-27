using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private float distTolerance;
    [SerializeField] private float newRotaWaitTime;

    private Coroutine _newRotaCoroutine;
    private WaitForSeconds _newRotaCalculateWaitTime;
    private Vector3 _targetPos;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void FindPath(Vector3 targetPos)
    {
        _targetPos = targetPos;

        grid.GridInitialize(Vector3.zero, _targetPos - transform.position);

        Debug.Log("FindPath:::" + _targetPos);

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
                if (!IsWalkableControl(neighbour.worldPosition) || closedSet.Contains(neighbour))
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
