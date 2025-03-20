using System;
using DG.Tweening;
using Tool;
using UnityEngine;

namespace TileStack
{
    public enum Direction
    {
        Forward,
        Backward,
        Left,
        Right,
        ForwardLeft,
        ForwardRight,
        BackwardLeft,
        BackwardRight
    }

    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private Direction _direction;

        [SerializeField]
        private Board _board;

        [SerializeField]
        private Transform _indicator;

        [SerializeField]
        private Vector2Int _currentPosition;

        [SerializeField]
        private float _duration = 1f;

        private Vector2Int DirectionVector => GetDirection();

        private bool _isMoving;

        private void Start()
        {
            UpdateIndicator();
        }

        [Button]
        public void UpdateIndicator()
        {
            _indicator.forward = new Vector3(DirectionVector.x, 0, DirectionVector.y);
        }

        [Button]
        public void Move()
        {
            if (_isMoving) return;
            DoMove();
        }

        private void DoMove()
        {
            var newPosition = _currentPosition + DirectionVector;
            if (!_board.IsInsideBoard(newPosition))
            {
                _isMoving = false;
                return;
            }

            _isMoving = true;
            var targetCell = _board.GetCell(newPosition);
            var targetPosition = targetCell.transform.position;
            targetPosition.y = transform.position.y;
            transform.DOMove(targetPosition, duration: _duration)
                     .SetEase(Ease.Linear)
                     .OnComplete(() =>
                      {
                          _currentPosition = newPosition;
                          DoMove();
                      });
        }

        private Vector2Int GetDirection()
        {
            return _direction switch
                   {
                       Direction.Forward => Vector2Int.up,
                       Direction.Backward => Vector2Int.down,
                       Direction.Left => Vector2Int.left,
                       Direction.Right => Vector2Int.right,
                       Direction.ForwardLeft => Vector2Int.up + Vector2Int.left,
                       Direction.ForwardRight => Vector2Int.up + Vector2Int.right,
                       Direction.BackwardLeft => Vector2Int.down + Vector2Int.left,
                       Direction.BackwardRight => Vector2Int.down + Vector2Int.right,
                       _ => Vector2Int.zero
                   };
        }
    }
}