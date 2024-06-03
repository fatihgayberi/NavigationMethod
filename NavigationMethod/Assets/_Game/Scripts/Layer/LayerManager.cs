using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wonnasmith;
using System;

[Serializable]
public class LayerManager : Singleton<LayerManager>
{
    public enum LayerType
    {
        NONE,

        LayerTerrain_LAYER
    }

    [Serializable]
    private class LayerDatas
    {
        public LayerType layerType;
        public LayerMask layerMask;
    }

    [SerializeField] private LayerDatas[] layerDatasArray;

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    public bool IsLayerEquals(int objectLayer, LayerType layerType)
    {
        LayerDatas layerDatas = GetLayerDatas(layerType);

        if (layerDatas == null)
        {
            return false;
        }

        int layerNum = Wonnasmith.LayerMaskExtensionMethods.LayerMask2Int(layerDatas.layerMask);

        return objectLayer.Equals(layerNum);
    }

    //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

    private LayerDatas GetLayerDatas(LayerType layerType)
    {
        if (layerType == LayerType.NONE)
        {
            return null;
        }

        if (layerDatasArray == null)
        {
            return null;
        }

        int layerDatasArrayLength = layerDatasArray.Length;

        for (int i = 0; i < layerDatasArrayLength; i++)
        {
            if (layerDatasArray[i] != null)
            {
                if (layerDatasArray[i].layerType == layerType)
                {
                    return layerDatasArray[i];
                }
            }
        }

        return null;
    }
}
