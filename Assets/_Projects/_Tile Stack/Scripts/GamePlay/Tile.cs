using System.Collections.Generic;
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
        BackwardRight,
        None
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
        private Transform _cube;

        [SerializeField]
        private Transform _jumpTarget;

        [SerializeField]
        private Vector2Int _currentPosition;

        [SerializeField]
        private float _duration = 1f;

        public Vector2Int CurrentPosition => _currentPosition;
        private Transform JumpTarget => _jumpTarget;
        private Vector2Int DirectionVector => _direction.GetDirectionVector();
        private readonly List<Tile> _stackedTiles = new();
        private Tween _tween;


        private bool _isMoving;

        private void OnValidate()
        {
            UpdateIndicator();
            UpdatePosition();
        }

        private void Start()
        {
            UpdateIndicator();
            UpdatePosition();
        }

        [Button]
        private void UpdateIndicator()
        {
            if (_direction == Direction.None)
            {
                _indicator.gameObject.SetActive(false);
            }
            else
            {
                _indicator.gameObject.SetActive(true);
                _indicator.forward = new Vector3(DirectionVector.x, 0, DirectionVector.y);
            }
        }

        private void UpdatePosition()
        {
            if (_board == null) return;
            transform.position = _board.GetCell(_currentPosition).transform.position;
        }

        [Button]
        public void ForceMove()
        {
            if (_isMoving) return;
            Move();
        }

        private void Move()
        {
            if (_board == null) return;
            var newPosition = _currentPosition + DirectionVector;
            if (_board.TryGetTileAt(newPosition, out var tile))
            {
                DoJump(newPosition, tile);
            }
            else
            {
                DoMove(newPosition);
            }
        }

        private void DoMove(Vector2Int newPosition)
        {
            if (!_board.IsInsideBoard(newPosition))
            {
                _isMoving = false;
                return;
            }

            _isMoving = true;
            var targetCell = _board.GetCell(newPosition);
            var targetPosition = targetCell.transform.position;
            targetPosition.y = transform.position.y;
            _tween = transform.DOMove(targetPosition, duration: _duration)
                              .SetEase(Ease.Linear)
                              .OnComplete(() => { OnDoneStep(newPosition, targetCell); });
        }

        private void DoJump(Vector2Int newPosition, Tile targetTile)
        {
            if (!_board.IsInsideBoard(newPosition))
            {
                _isMoving = false;
                return;
            }

            _isMoving = true;
            var targetCell = _board.GetCell(newPosition);
            var targetPosition = targetCell.transform.position;
            if (targetTile != null)
                targetPosition = targetTile.JumpTarget.transform.position;
            targetPosition.y = transform.position.y;
            _tween = transform.DOJump(targetPosition, jumpPower: 0.4f, numJumps: 1, duration: _duration)
                              .SetEase(Ease.Linear)
                              .OnComplete(() => { OnDoneStep(newPosition, targetCell, targetTile); });
        }

        private void OnDoneStep(Vector2Int newPosition, Cell targetCell, Tile targetTile = null)
        {
            _currentPosition = newPosition;
            if (targetCell.Direction != Direction.None)
            {
                _direction = targetCell.Direction;
                UpdateIndicator();
            }

            if (targetTile != null)
            {
                StackTile(targetTile);
            }

            Move();
        }

        private void StackTile(Tile targetTile)
        {
            _board.TileMap.Remove(_currentPosition);
            _stackedTiles.Add(targetTile);
            var cubePosition = _cube.localPosition;
            cubePosition.y = GetStackedHeight(0);
            _cube.localPosition = cubePosition;
            targetTile.transform.SetParent(transform, true);
            targetTile.transform.localPosition = Vector3.zero;

            for (var i = 0; i < _stackedTiles.Count; i++)
            {
                var height = GetStackedHeight(i + 1);
                _stackedTiles[i].transform.localPosition = new Vector3(0, height, 0);
            }
        }

        private float GetStackedHeight(int index)
        {
            return (_stackedTiles.Count - index) * _jumpTarget.localPosition.y;
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        private void OnMouseDown()
        {
            if (_isMoving) return;
            if (_direction == Direction.None) return;
            Move();
        }
    }
}