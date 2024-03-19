using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;

namespace HHG.PathfindingSystem
{
    public class Pathfinding : MonoBehaviour
    {
        private PathRequestManager requestManager;
        private PathfindGrid grid;

        public PathEditorData pathEditorData = new PathEditorData();

        private void Awake()
        {
            requestManager = GetComponent<PathRequestManager>();
            grid = GetComponent<PathfindGrid>();

            // for (int i = 0; i < 200; i++)
            // {
            //     FindPath(Vector3.zero, Vector3.zero);
            // }
        }

        public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        {
            FindPath(startPos, targetPos, true);
        }

        private void FindPath(Vector3 startPos, Vector3 targetPos, bool isUnit = false)
        {
            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = grid.grid[UnityEngine.Random.Range(0, 197), UnityEngine.Random.Range(0, 197)];//.NodeFromWorldPoint(startPos);
            Node targetNode = grid.grid[UnityEngine.Random.Range(0, 197), UnityEngine.Random.Range(0, 197)];//.NodeFromWorldPoint(targetPos);

            Debug.Log("startNode:::" + startNode.worldPosition);
            Debug.Log("targetNode:::" + targetNode.worldPosition);

            if (startNode.walkable && targetNode.walkable)
            {
                Heap<Node> openSet = new Heap<Node>(grid.MaxSize);

                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    pathEditorData.FindPathWhileCount++;

                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode.gridX == targetNode.gridX && currentNode.gridY == targetNode.gridY)
                    {
                        Debug.Log("targetNode:::POS:::" + currentNode.worldPosition);
                        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~YOL BULUNDU~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        pathSuccess = true;
                        break;
                    }

                    List<Node> localNeighbours = grid.GetNeighbours(currentNode);

                    for (int i = 0; i < localNeighbours.Count; i++)
                    {
                        Node neighbour = localNeighbours[i];

                        pathEditorData.NeighbourForeachCount++;
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }

                            localNeighbours[i] = neighbour;
                        }
                    }

                    GridNodeUpdate(localNeighbours);
                }
            }

            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
            }

            if (isUnit)
            {
                requestManager.FinishedProcessingPath(waypoints, pathSuccess);
            }
        }

        private void GridNodeUpdate(List<Node> nodes)
        {
            foreach (var item in nodes)
            {
                grid.grid[item.gridX, item.gridY] = item;
            }
        }

        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        private Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    [Serializable]
    public class PathEditorData
    {
        public int GridGeneratorForeachCount;
        public float GridGeneratorForeachTime;
        public int FindPathWhileCount;
        public float FindPathWhileTime;
        public int OpenListForCount;
        public float OpenListTime;
        public int NeighbourForeachCount;
        public int NeighbourForeachTime;
        public int RetracePathWhileCount;
    }
}