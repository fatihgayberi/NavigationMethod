using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerInputMoveController : MonoBehaviour
{
    public static event SeekerManager.SeekerManagerSeekerCharacterMovePathGenerator SeekerCharacterMovePathGenerator;

    public List<Pathfinding> _selectPathfindingList;
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
            if (_selectPathfindingList != null)
            {
                _selectPathfindingList.Clear();
            }
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
                    if (_selectPathfindingList == null)
                    {
                        _selectPathfindingList = new List<Pathfinding>();
                    }

                    Pathfinding clickPathfinding = raycastHit.transform.gameObject.GetComponent<Pathfinding>();

                    if (_selectPathfindingList.Contains(clickPathfinding))
                    {
                        return;
                    }
                    else if (clickPathfinding != null)
                    {
                        _selectPathfindingList.Add(clickPathfinding);
                    }

                    if (_selectPathfindingList.Count != 0)
                    {
                        if (LayerManager.Instance.IsLayerEquals(raycastHit.transform.gameObject.layer, LayerManager.LayerType.LayerTerrain_LAYER))
                        {
                            targetPos = raycastHit.point;

                            SeekerCharacterMovePathGenerator?.Invoke(_selectPathfindingList, targetPos);
                        }
                    }
                }
            }
        }
    }
}