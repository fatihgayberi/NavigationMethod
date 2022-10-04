using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SeekerInputMoveController : MonoBehaviour
{
    public static event SeekerManager.SeekerManagerSeekerCharacterMovePathGenerator SeekerCharacterMovePathGenerator;

    private List<SeekerPathfinding> _selectSeekerPathfindingList;
    
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
            if (_selectSeekerPathfindingList != null)
            {
                _selectSeekerPathfindingList.Clear();
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
                    if (_selectSeekerPathfindingList == null)
                    {
                        _selectSeekerPathfindingList = new List<SeekerPathfinding>();
                    }

                    SeekerPathfinding clickSeekerPathfinding = raycastHit.transform.gameObject.GetComponent<SeekerPathfinding>();

                    if (_selectSeekerPathfindingList.Contains(clickSeekerPathfinding))
                    {
                        return;
                    }
                    else if (clickSeekerPathfinding != null)
                    {
                        _selectSeekerPathfindingList.Add(clickSeekerPathfinding);
                    }

                    if (_selectSeekerPathfindingList.Count != 0)
                    {
                        if (LayerManager.Instance.IsLayerEquals(raycastHit.transform.gameObject.layer, LayerManager.LayerType.LayerTerrain_LAYER))
                        {
                            SeekerCharacterMovePathGenerator?.Invoke(_selectSeekerPathfindingList, raycastHit.point);
                        }
                    }
                }
            }
        }
    }
}