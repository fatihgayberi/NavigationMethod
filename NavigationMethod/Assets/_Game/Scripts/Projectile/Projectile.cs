using System;
using UnityEngine;

namespace Wonnasmith.ProjectileMotion
{
    [Serializable]
    public class Projectile
    {
        public /*readonly*/ Vector3[] _bernsteinPolynomalPos = new Vector3[3];

        private Vector3 _startPos;
        private Vector3 _targetPos;
        public float _x;

        private float X { get => _x; set => _x = value; }

        public Projectile(Vector3 startPos, Vector3 targetPos, float height)
        {
            ThrowInitialize(startPos, targetPos, height, 1f);
        }

        public Projectile(Vector3 startPos, Vector3 targetPos, float height, float reverseDirection)
        {
            ThrowInitialize(startPos, targetPos, height, reverseDirection);
        }

        private void ThrowInitialize(Vector3 startPos, Vector3 targetPos, float height, float reverseDirection)
        {
            X = 0;

            _startPos = startPos;
            _targetPos = targetPos;

            Vector3 midPos = GetMidPoint(_startPos, _targetPos, height, reverseDirection);

            _bernsteinPolynomalPos[0] = _startPos;
            _bernsteinPolynomalPos[1] = midPos;
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

        public Vector3 BernsteinPositionCalculator(float currentPercent)
        {
            if (_bernsteinPolynomalPos == null) return _startPos;
            if (_bernsteinPolynomalPos.Length == 0) return _startPos;

            Vector3 bernsteinPos = Vector3.zero;
            int n = _bernsteinPolynomalPos.Length - 1;

            if (currentPercent <= 0) return _bernsteinPolynomalPos[0];
            if (currentPercent >= 1) return _bernsteinPolynomalPos[n];

            for (int v = 0; v < _bernsteinPolynomalPos.Length; v++)
            {
                bernsteinPos += _bernsteinPolynomalPos[v] * currentPercent.Bernstein(n, v);
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
            if (X >= 1)
            {
                return (_targetPos, true);
            }
            else
            {
                X = Mathf.MoveTowards(X, 1, deltaTimeType * speed);
                return (BernsteinPositionCalculator(X), false);
            }
        }
    }
}