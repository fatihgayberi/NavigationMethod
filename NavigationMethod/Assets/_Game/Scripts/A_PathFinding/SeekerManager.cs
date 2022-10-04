using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wonnasmith;

public class SeekerManager : Singleton<SeekerManager>
{
    public delegate void SeekerManagerSeekerCharacterMovePathGenerator(SeekerPathfinding seekerPathfinding, Vector3 targetPos);

    enum SeekerType
    {
        NONE,

        SeekerType_1
    }

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

    private void OnSeekerCharacterMovePathGenerator(SeekerPathfinding seekerPathfinding, Vector3 targetPos)
    {
        if (seekerPathfinding == null)
        {
            return;
        }

        seekerPathfinding.FindPath(targetPos);
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
