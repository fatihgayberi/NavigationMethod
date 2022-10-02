using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wonnasmith;

public class SeekerManager : Singleton<SeekerManager>
{
    public delegate void SeekerManagerSeekerCharacterMovePathGenerator(Pathfinding pathfinding, Vector3 targetPos);

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void OnEnable()
    {
        SeekerInputMoveController.SeekerCharacterMovePathGenerator += OnSeekerCharacterMovePathGenerator;
    }

    private void OnDisable()
    {
        SeekerInputMoveController.SeekerCharacterMovePathGenerator -= OnSeekerCharacterMovePathGenerator;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void OnSeekerCharacterMovePathGenerator(Pathfinding pathfinding, Vector3 targetPos)
    {
        if (pathfinding == null)
        {
            return;
        }

        pathfinding.FindPath(targetPos);
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    /// <summary>
    /// Test için kullanılır
    /// </summary>
    public void GizmosClear()
    {

    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]
}
