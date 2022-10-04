﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SeekerPathfinding : MonoBehaviour
{
    private SeekerGrid _seekerGrid;
    private Coroutine _newRotaCoroutine;
    private WaitForSeconds _newRotaCalculateWaitTime;
    private Vector3 _targetPos;
    private SeekerManager.SeekerType _seekerType;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SetSeekerGrid(SeekerGrid seekerGrid)
    {
        _seekerGrid = seekerGrid;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SetSeekerType(SeekerManager.SeekerType seekerType)
    {
        _seekerType = seekerType;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void FindPath(Vector3 targetPos)
    {
        _targetPos = targetPos;

        _seekerGrid.SeekerGridInitialize(Vector3.zero, _targetPos - transform.position);

        Node startNode = _seekerGrid.NodeFromWorldPoint(Vector3.zero);
        Node targetNode = _seekerGrid.NodeFromWorldPoint(_targetPos - transform.position);

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
                _seekerGrid.SeekerGridExpand();
                return;
            }

            foreach (Node neighbour in _seekerGrid.GetNeighbours(node))
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                if (!IsWalkableControl(neighbour.worldPosition))
                {
                    continue;
                }

                if (!IsVerySlopeControl(neighbour, node))
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

        if (dist <= SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekerType).distTolerance)
        {
            Debug.Log("DUR YOLCU 07 zaten çok yakındasın ilerlemene gerek yok");
            return;
        }

        Debug.Log("Yeni rota hesaplanıyor");

        if (SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekerType).newRotaWaitTime == 0)
        {
            Debug.Log("direkt Yeni rota hesaplanıyor");
            FindPath(_targetPos);
            return;
        }

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
            _newRotaCalculateWaitTime = new WaitForSeconds(SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekerType).newRotaWaitTime);
        }

        Debug.Log("Yeni rota hesaplamak için zaman geçmesi bekleniyor::" + SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekerType).newRotaWaitTime);
        yield return _newRotaCalculateWaitTime;

        FindPath(_targetPos);
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private bool IsWalkableControl(Vector3 worldPoint)
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
        float slope = SlopeCalculator(neighbour, node);

        if (slope <= SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekerType).maxSeekerSlope)
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

        // Debug.Log("delta_Y:::" + delta_Y);
        // Debug.Log("dist:::" + dist);
        // Debug.Log("slope:::" + (delta_Y / dist));
        // Debug.Log("slopePercent:::" + Wonnasmith.FloatExtensionMethods.FloatRemap(slope, 0, 1, 0, 100));

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
        Debug.Log("<color=green>:::YOL BULUNDU:::</color>");

        _seekerGrid.pathTEST = path;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.seekerGridX - nodeB.seekerGridX);
        int dstY = Mathf.Abs(nodeA.seekerGridY - nodeB.seekerGridY);

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