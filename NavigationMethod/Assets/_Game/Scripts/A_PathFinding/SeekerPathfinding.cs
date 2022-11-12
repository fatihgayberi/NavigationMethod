using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Dreamteck.Splines;

public class SeekerPathfinding : MonoBehaviour
{
    private SeekerGrid _seekerGrid;
    private Coroutine _newRotaCoroutine;
    private WaitForSeconds _newRotaCalculateWaitTime;
    private Vector3 _targetPos;

    private SplineFollower _splineFollower;
    private Spline _spline = new Spline(Spline.Type.CatmullRom);

    private int _seekderDataIdx;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SetSeekerGrid(SeekerGrid seekerGrid)
    {
        _seekerGrid = seekerGrid;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void SetSeekerDataIdx(int seekderDataIdx)
    {
        _seekderDataIdx = seekderDataIdx;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public void FindPath(Vector3 targetPos)
    {
        _splineFollower = gameObject.GetComponent<SplineFollower>();

        if (_splineFollower == null)
        {

            if (_splineFollower == null)
            {
                return;
            }
        }

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
        SplineFollower splineFollower = gameObject.GetComponent<SplineFollower>();

        splineFollower.follow = false;

        float dist = Vector3.Distance(_targetPos, transform.position);

        if (dist <= SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekderDataIdx).distTolerance)
        {
            Debug.Log("DUR YOLCU 07 zaten çok yakındasın ilerlemene gerek yok");
            return;
        }

        Debug.Log("Yeni rota hesaplanıyor");

        if (SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekderDataIdx).newRotaWaitTime == 0)
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
            _newRotaCalculateWaitTime = new WaitForSeconds(SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekderDataIdx).newRotaWaitTime);
        }

        Debug.Log("Yeni rota hesaplamak için zaman geçmesi bekleniyor::" + SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekderDataIdx).newRotaWaitTime);
        yield return _newRotaCalculateWaitTime;

        FindPath(_targetPos);
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private bool IsWalkableControl(Vector3 worldPoint)
    {
        Collider[] hitcollider = Physics.OverlapSphere(worldPoint, SeekerManager.Instance.GetNodeRadius(_seekderDataIdx));

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

        if (slope <= SeekerManager.Instance.GetSeekerSeekerPathfindingDatas(_seekderDataIdx).maxSeekerSlope)
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

        _seekerGrid.pathTEST = path;

        SplineEdit(path);

        Debug.Log("<color=green>:::YOL BULUNDU YÜRÜ YA KULUM:::</color>");
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public Quaternion prevTargetRotatetion;
    public Vector3 prevTargetMotionAngle;

    private void SplineEdit(List<Node> path)
    {
        if (_splineFollower == null)
        {
            return;
        }

        prevTargetRotatetion = transform.rotation;
        prevTargetMotionAngle = _splineFollower.motion.rotationOffset;

        Vector2 currentMotionOffset = _splineFollower.motion.offset;

        _splineFollower.follow = false;

        _spline.points = new SplinePoint[path.Count + 1];
        _spline.points[0] = new SplinePoint(transform.position);

        for (int i = 1; i < path.Count + 1; i++)
        {
            _spline.points[i] = new SplinePoint(path[i - 1].worldPosition);
        }

        _splineFollower.spline.SetPoints(_spline.points);
        Vector3 targetTangent = _splineFollower.spline.GetPointNormal(0);
        Vector3 targetTangent2 = _splineFollower.spline.GetPointTangent2(0);

        _splineFollower.motion.offset = currentMotionOffset;

        SplineSample asdf = null;

        // _splineFollower.Project(transform.position, (SplineSample)_spline);
        _splineFollower.SetPercent(0);

        _splineFollower.spline.RebuildImmediate();
        _splineFollower.RebuildImmediate();

        _splineFollower.follow = true;

        // Debug.Log("eulerAnglesThis::: " + transform.rotation.eulerAngles);
        // Debug.Log("targetTangent::: " + targetTangent);
        // Debug.Log("targetTangent2::: " + targetTangent2);
        // _splineFollower.motion.rotationOffset = transform.rotation.eulerAngles * -1;



        StartCoroutine(TestFunc());
    }

    private IEnumerator TestFunc()
    {

        yield return null;


        yield return null;
        yield return null;
        yield return null;
        yield return null;

        // Debug.Log("------------eulerAnglesThis::: " + transform.rotation.eulerAngles);
        yield break;

        // Debug.Log(":::" + Quaternion.Euler(Vector3.one * 360 - transform.rotation.eulerAngles).eulerAngles);
        // _splineFollower.motion.rotationOffset = Quaternion.Euler(Vector3.one * 360 - transform.rotation.eulerAngles).eulerAngles;//Quaternion.Euler(Vector3.one * 360 - transform.rotation.eulerAngles).eulerAngles;

        // _splineFollower.RebuildImmediate();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public float rotateSpeed = 250;
    public bool isRotateTest = true;

    // test halinde hala bazı eklsiklikleri var
    private void Update()
    {
        if (isRotateTest)
        {
            return;
        }

        if (_splineFollower == null)
        {
            return;
        }

        if (_splineFollower.follow)
        {
            _splineFollower.motion.rotationOffset = Quaternion.RotateTowards(Quaternion.Euler(_splineFollower.motion.rotationOffset), Quaternion.Euler(Vector3.zero), rotateSpeed * Time.deltaTime).eulerAngles;
        }
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
