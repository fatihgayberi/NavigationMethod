using UnityEngine;
using System.Collections;

namespace HHG.PathfindingSystem
{
    public class Unit : MonoBehaviour
    {
        public Transform _target;
        private float _speed = 20;
        private Vector3[] _path;
        private int _targetIndex;

        private UnitStatus _unitStatus;
        private Coroutine _followPathCoroutine;

        [ContextMenu("UnitPathRequest")]
        private void UnitPathRequest()
        {
            PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);
        }

        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            // yol var mi diye kontrol yapildi
            if (!pathSuccessful) return;

            _path = newPath;
            _targetIndex = 0;

            if (_followPathCoroutine != null)
            {
                StopCoroutine(FollowPath());
            }

            _followPathCoroutine = StartCoroutine(FollowPath());
        }

        /// <summary> Player target a dogru hareket ediyor </summary>
        private IEnumerator FollowPath()
        {
            Vector3 currentWaypoint = _path[0];

            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    _targetIndex++;
                    if (_targetIndex >= _path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = _path[_targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);

                yield return null;
            }
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (_path == null) return;

            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(_path[i], Vector3.one);

                if (i == _targetIndex)
                {
                    Debug.DrawLine(transform.position, _path[i], Color.black, 5000);
                }
                else
                {
                    Debug.DrawLine(_path[i - 1], _path[i], Color.black, 5000);
                }
            }
        }
#endif
    }
}
