﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HHG.PathfindingSystem
{
    public class PathfindGrid : MonoBehaviour
    {
        public bool displayGridGizmos;
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public Node[,] grid;

        private float nodeDiameter;
        private int gridSizeX, gridSizeY;

        private void Awake()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            CreateGrid();
        }

        public int MaxSize
        {
            get
            {
                return gridSizeX * gridSizeY;
            }
        }

        public int GridGeneratorForeachCount;

        private void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                    if (!walkable)
                    {
                        worldPoint += Vector3.up * 5f;
                    }
                    GridGeneratorForeachCount++;

                    grid[x, y] = new Node(true, worldPoint, x, y);

                    Debug.Log("worldPoint:::" + worldPoint);
                }
            }
        }

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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, new Vector3(nodeDiameter - .1f, 0.01f, nodeDiameter - .1f));
                }
            }
        }
    }
}