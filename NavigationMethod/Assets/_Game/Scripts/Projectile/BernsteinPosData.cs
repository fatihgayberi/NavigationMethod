using System;
using UnityEngine;

namespace Wonnasmith.ProjectileMotion
{
    [Serializable]
    public class BernsteinPosData
    {
        private Vector3 pos = Vector3.negativeInfinity;
        private Transform dynamicPos = null;

        public BernsteinPosData(Vector3 pos)
        {
            this.pos = pos;
        }

        public BernsteinPosData(Transform dynamicPos)
        {
            this.dynamicPos = dynamicPos;
        }

        public Vector3 GetPos()
        {
            if (dynamicPos != null) return dynamicPos.position;
            else if (pos != Vector3.negativeInfinity) return pos;
            else return Vector3.zero;
        }
    }
}