using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wonnasmith;

public class TerrainsManager : Singleton<TerrainsManager>
{
    [SerializeField] private LayerMask layerMaskRay;
    [SerializeField] private float rayHeight;

    private Vector3 _rayOffset;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private void Start()
    {
        _rayOffset = Vector3.up * rayHeight / 2;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public float GetTerrainSampleHeight(Vector3 pos)
    {
        //Get the current terrain
        Terrain terrain = GetTerrain(pos);

        if (terrain == null)
        {
            return pos.y;
        }

        return terrain.transform.position.y + terrain.SampleHeight(pos); ;
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private Terrain GetTerrain(Vector3 pos)
    {
        Vector3 startPos = pos + _rayOffset;
        Vector3 endPos = pos - _rayOffset;

        RaycastHit hitInfo;

        if (Physics.Raycast(startPos, endPos, out hitInfo, 200f, layerMaskRay))
        {
            if (hitInfo.collider != null)
            {
                Terrain terrain = hitInfo.collider.gameObject.GetComponent<Terrain>();

                if (terrain == null)
                {
                    return null;
                }
                else
                {
                    return terrain;
                }
            }
        }

        return null;
    }
}