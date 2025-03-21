using System;
using UnityEngine;

namespace TileStack
{
    [Serializable]
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Direction _direction;
        [SerializeField] private Transform _indicator;
        public Vector2Int Position { get; set; }
        public Direction Direction => _direction;

        private void OnValidate()
        {
            if (_direction == Direction.None)
            {
                _indicator.gameObject.SetActive(false);
                return;
            }
            else
            {
                _indicator.gameObject.SetActive(true);
            }

            if (_indicator == null) return;
            var directionVector = _direction.GetDirectionVector();
            _indicator.forward = new Vector3(directionVector.x, 0, directionVector.y);
        }
    }
}