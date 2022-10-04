using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerController : MonoBehaviour
{
    [SerializeField] private SeekerManager.SeekerType seekerType;

    private SeekerPathfinding _seekerPathfinding;
    private SeekerGrid _seekerGrid;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void Awake()
    {
        SeekerGridInitialize();
        SeekerPathfindingInitialize();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void SeekerGridInitialize()
    {
        _seekerGrid = gameObject.AddComponent<SeekerGrid>();
        _seekerGrid.SetSeekerType(seekerType);
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void SeekerPathfindingInitialize()
    {
        _seekerPathfinding = gameObject.AddComponent<SeekerPathfinding>();
        _seekerPathfinding.SetSeekerGrid(_seekerGrid);
        _seekerPathfinding.SetSeekerType(seekerType);
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
            if (_seekerPathfinding == null)
            {
                return;
            }

            _seekerPathfinding.SeekerDetecetNavObs();
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
