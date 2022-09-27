using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerController : MonoBehaviour
{
    [SerializeField] private Pathfinding pathfinding;

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
            if (pathfinding == null)
            {
                return;
            }

            pathfinding.SeekerDetecetNavObs();
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
