using System;
using UnityEngine;

namespace Wonnasmith.ProjectileMotion
{
    [Serializable]
    public class ProjectileDynamic
    {
        public BernsteinPosData[] _bernsteinPolynomalPos = new BernsteinPosData[3];

        private BernsteinPosData _startPos;
        private BernsteinPosData _targetPos;
        public float _x;

        public ProjectileDynamic(BernsteinPosData startPos, BernsteinPosData targetPos, float height)
        {
            ThrowInitialize(startPos, targetPos, height, 1f);
        }

        public ProjectileDynamic(BernsteinPosData startPos, BernsteinPosData targetPos, float height, float reverseDirection)
        {
            ThrowInitialize(startPos, targetPos, height, reverseDirection);
        }

        private void ThrowInitialize(BernsteinPosData startPos, BernsteinPosData targetPos, float height, float reverseDirection)
        {
            _x = 0;

            _startPos = startPos;
            _targetPos = targetPos;

            Vector3 midPos = GetMidPoint(startPos.GetPos(), targetPos.GetPos(), height, reverseDirection);

            if (midPos == Vector3.negativeInfinity) return;

            _bernsteinPolynomalPos[0] = _startPos;
            _bernsteinPolynomalPos[1] = new BernsteinPosData(midPos);
            _bernsteinPolynomalPos[2] = _targetPos;
        }

        /// <summary>
        /// ikizkenar ucgen olusturup buldugu icin tepe noktasi hesaplanarak bulunabilir
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="distance"></param>
        /// <param name="reverseDirection"></param>
        /// <returns></returns>
        private Vector3 GetMidPoint(Vector3 startPos, Vector3 targetPos, float distance, float reverseDirection)
        {
            Vector3 midPoint = (startPos + targetPos) / 2; // Tabanin orta noktasi

            // Yukseklik vektorunu hesapla
            Vector3 heightVector = new Vector3(targetPos.y - startPos.y, -(targetPos.x - startPos.x)).normalized * -distance * reverseDirection;

            // Tepe noktasini hesapla
            Vector3 apexPoint = midPoint + heightVector;

            return apexPoint;
        }

        private Vector3 BernsteinPositionCalculator(float currentPercent)
        {
            if (_bernsteinPolynomalPos == null || _bernsteinPolynomalPos.Length == 0)
            {
                if (_startPos != null && _startPos.GetPos() != null) return _startPos.GetPos();
                else return Vector3.zero;
            }

            Vector3 bernsteinPos = Vector3.zero;
            int n = _bernsteinPolynomalPos.Length - 1;

            if (currentPercent <= 0)
            {
                if (_startPos != null && _startPos.GetPos() != null) return _startPos.GetPos();
                else return Vector3.zero;
            }
            if (currentPercent >= 1)
            {
                if (_targetPos != null && _targetPos.GetPos() != null) return _targetPos.GetPos();
                else return Vector3.zero;
            }

            for (int v = 0; v < _bernsteinPolynomalPos.Length; v++)
            {
                if (_bernsteinPolynomalPos[v] == null) continue;

                if (_bernsteinPolynomalPos[v].GetPos() != null)
                {
                    bernsteinPos += _bernsteinPolynomalPos[v].GetPos() * currentPercent.Bernstein(n, v);
                }
            }

            return bernsteinPos;
        }

        /// <summary>
        /// <para>verilen hizda bernstein polinomunu Update ile cizer</para> 
        /// <para>item2 true doner ise hareketi tamamlamistir</para> 
        /// </summary>
        public (Vector3, bool) BernsteinMoveUpdate(float speed) { return BernsteinMove(speed, Time.deltaTime); }

        /// <summary>
        /// <para>verilen hizda bernstein polinomunu FixedUpdate ile cizer</para> 
        /// <para>item2 true doner ise hareketi tamamlamistir</para> 
        /// </summary>        
        public (Vector3, bool) BernsteinMoveFixedUpdate(float speed) { return BernsteinMove(speed, Time.fixedDeltaTime); }

        private (Vector3, bool) BernsteinMove(float speed, float deltaTimeType)
        {
            if (_x >= 1)
            {
                if (_targetPos != null && _targetPos.GetPos() != null) return (_targetPos.GetPos(), true);
                else return (Vector3.zero, true);
            }
            else
            {
                _x = Mathf.MoveTowards(_x, 1, deltaTimeType * speed);
                return (BernsteinPositionCalculator(_x), false);
            }
        }
    }
}