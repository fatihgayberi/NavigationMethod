using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerController : MonoBehaviour
{
    [SerializeField] private SeekerManager.SeekerType seekerType;

    private Pathfinding _pathfinding;
    private SeekerGrid grid;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void Start()
    {
        GridInitialze();
        PathfindingInitialze();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void GridInitialze()
    {
        grid = gameObject.AddComponent<SeekerGrid>();

        grid.SetSeekerType(seekerType);
        grid.SetTerrain(GetTerrain());
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void PathfindingInitialze()
    {
        _pathfinding = gameObject.AddComponent<Pathfinding>();
        _pathfinding.SetSeekerType(seekerType);

        if (grid != null)
        {
            _pathfinding.SetGrid(grid);
        }
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private Terrain GetTerrain()
    {
        Collider[] hitcollider = Physics.OverlapSphere(transform.position, SeekerManager.Instance.GetTerrainFindRadius());

        if (hitcollider == null)
        {
            return null;
        }

        if (hitcollider.Length == 0)
        {
            return null;
        }

        int hitcolliderLength = hitcollider.Length;

        for (int i = 0; i < hitcolliderLength; i++)
        {
            Terrain terrain = hitcollider[i].gameObject.GetComponent<Terrain>();

            if (terrain != null)
            {
                return terrain;
            }
        }

        return null;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void OnTriggerEnter(Collider other)
    {
        // if (!GameManager.IsPause())
        // {
        //     return;
        // }

        if (other == null)
        {
            return;
        }

        if (other.gameObject == null)
        {
            return;
        }

        if (IsSeekerWalkDetectionNavObs(other.gameObject))
        {
            if (_pathfinding == null)
            {
                return;
            }

            _pathfinding.SeekerDetecetNavObs();
        }
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private bool IsSeekerWalkDetectionNavObs(GameObject detectionObject)
    {
        if (detectionObject == null)
        {
            return false;
        }

        NavObs navObs = detectionObject.GetComponent<NavObs>();

        if (navObs == null)
        {
            return false;
        }

        return true;
    }
}
