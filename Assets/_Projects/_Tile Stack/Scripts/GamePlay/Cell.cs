using System;
using ObjectPooler;
using UnityEngine;

namespace TileStack
{
    [Serializable]
    public class Cell : PoolableMonoBehaviourBase
    {
        [SerializeField]
        private Direction _direction;

        [SerializeField]
        private GameObject _indicator;

        public Direction Direction => _direction;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (this == null) return;
            if (_direction == Direction.None)
            {
                _indicator.SetActive(false);
                return;
            }
            else
            {
                _indicator.SetActive(true);
            }

            if (_indicator == null) return;
            var directionVector = _direction.GetDirectionVector();
            _indicator.transform.forward = new Vector3(directionVector.x, 0, directionVector.y);
#endif
        }
    }
}