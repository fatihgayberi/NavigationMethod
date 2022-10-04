using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerInputMoveController : MonoBehaviour
{
    public static event SeekerManager.SeekerManagerSeekerCharacterMovePathGenerator SeekerCharacterMovePathGenerator;

    public SeekerPathfinding _selectSeekerPathfinding;
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
            _selectSeekerPathfinding = null;
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
                    SeekerPathfinding clickSeekerPathfinding = raycastHit.transform.gameObject.GetComponent<SeekerPathfinding>();

                    if (_selectSeekerPathfinding == null)
                    {
                        _selectSeekerPathfinding = clickSeekerPathfinding;
                    }
                    else if (clickSeekerPathfinding != null && _selectSeekerPathfinding != clickSeekerPathfinding)
                    {
                        _selectSeekerPathfinding = clickSeekerPathfinding;
                    }

                    if (clickSeekerPathfinding == null)
                    {
                        if (LayerManager.Instance.IsLayerEquals(raycastHit.transform.gameObject.layer, LayerManager.LayerType.LayerTerrain_LAYER))
                        {
                            targetPos = raycastHit.point;

                            SeekerCharacterMovePathGenerator?.Invoke(_selectSeekerPathfinding, targetPos);
                        }
                    }
                }
            }
        }
    }
}