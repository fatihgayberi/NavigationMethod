using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerController : MonoBehaviour
{
    [SerializeField] private string strSeekerLabel;

    private SeekerPathfinding _seekerPathfinding;
    private SeekerGrid _seekerGrid;

    private void Start()
    {
        int seekderIdx = SeekerManager.Instance.GetSeekerLabelIdx(strSeekerLabel);

        SeekerGridInitialize(seekderIdx);
        SeekerPathfindingInitialize(seekderIdx);
    }

    private void SeekerGridInitialize(int seekderIdx)
    {
        _seekerGrid = gameObject.AddComponent<SeekerGrid>();
        _seekerGrid.SetSeekerDataIdx(seekderIdx);
    }

    private void SeekerPathfindingInitialize(int seekderIdx)
    {
        _seekerPathfinding = gameObject.AddComponent<SeekerPathfinding>();
        _seekerPathfinding.SetSeekerGrid(_seekerGrid);
        _seekerPathfinding.SetSeekerDataIdx(seekderIdx);
    }

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
