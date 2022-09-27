using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerInputMoveController : MonoBehaviour
{
    public static event SeekerManager.SeekerManagerSeekerCharacterMovePathGenerator SeekerCharacterMovePathGenerator;

    public Pathfinding _selectPathfinding;
    public Vector3 targetPos;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void Update()
    {
        // if (!GameManager.IsPause())
        // {
        //     return;
        // }

        InputSelectedSeekerBreaker();
        InputMoveSelect();
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void InputSelectedSeekerBreaker()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _selectPathfinding = null;
        }
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void InputMoveSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    Pathfinding clickPathfinding = raycastHit.transform.gameObject.GetComponent<Pathfinding>();

                    if (_selectPathfinding == null)
                    {
                        _selectPathfinding = clickPathfinding;
                    }
                    else if (clickPathfinding != null && _selectPathfinding != clickPathfinding)
                    {
                        _selectPathfinding = clickPathfinding;
                    }

                    if (clickPathfinding == null)
                    {
                        if (LayerManager.Instance.IsLayerEquals(raycastHit.transform.gameObject.layer, LayerManager.LayerType.LayerTerrain_LAYER))
                        {
                            targetPos = raycastHit.point;

                            SeekerCharacterMovePathGenerator?.Invoke(_selectPathfinding, targetPos);
                        }
                    }
                }
            }
        }
    }
}