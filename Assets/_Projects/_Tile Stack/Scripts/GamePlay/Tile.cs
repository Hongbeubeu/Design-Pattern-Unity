using System.Collections.Generic;
using DG.Tweening;
using ObjectPooler;
using Tool;
using UnityEditor;
using UnityEngine;

namespace TileStack
{
    public enum Direction
    {
        None,
        Forward,
        Backward,
        Left,
        Right,
        ForwardLeft,
        ForwardRight,
        BackwardLeft,
        BackwardRight
    }

    public class Tile : PoolableMonoBehaviourBase
    {
        [SerializeField]
        private Board _board;

        [SerializeField]
        private Transform _indicator;

        [SerializeField]
        private Transform _cube;

        [SerializeField]
        private Transform _jumpTarget;

        [SerializeField]
        private Collider _collider;

        [SerializeField]
        private Vector2Int _currentPosition;

        [SerializeField]
        private float _speed = 1f;

        [SerializeField]
        private TileData _tileData;


        public Board Board
        {
            get => _board;
            set => _board = value;
        }

        public TileData TileData
        {
            get => _tileData;
            set => _tileData = value;
        }

        public Vector2Int CurrentPosition => _currentPosition;
        private Transform JumpTarget => _jumpTarget;

        private Direction Direction
        {
            get => _tileData.direction;
            set => _tileData.direction = value;
        }

        private Vector2Int DirectionVector => _tileData.direction.GetDirectionVector();
        private readonly List<Tile> _stackedTiles = new();
        private Tween _tween;


        private bool _isMoving;

        private void Start()
        {
            UpdateIndicator();
            UpdatePosition();
        }

        [Button]
        public void UpdateIndicator()
        {
            if (Direction == Direction.None)
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
            var cell = _board.GetCell(_currentPosition);
            if (cell == null) return;
            transform.position = cell.transform.position;
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
            if (!_board.HasCell(newPosition))
            {
                ResetTile();
                return;
            }

            _isMoving = true;
            var targetCell = _board.GetCell(newPosition);
            var targetPosition = targetCell.transform.position;
            targetPosition.y = transform.position.y;

            var duration = Vector3.Distance(transform.position, targetPosition) / _speed;
            _tween = transform.DOMove(targetPosition, duration: duration)
                              .SetEase(Ease.Linear)
                              .OnComplete(() => { OnDoneStep(newPosition, targetCell); });
        }

        private void DoJump(Vector2Int newPosition, Tile targetTile)
        {
            if (!_board.HasCell(newPosition))
            {
                ResetTile();
                return;
            }

            _isMoving = true;
            var targetCell = _board.GetCell(newPosition);
            var targetPosition = targetCell.transform.position;
            var duration = Vector3.Distance(transform.position, targetPosition) / _speed;
            if (targetTile != null)
                targetPosition = targetTile.JumpTarget.transform.position;
            targetPosition.y = transform.position.y;
            var jumpPower = targetPosition.y - transform.position.y + 0.1f;

            _tween = transform.DOJump(targetPosition, jumpPower: jumpPower, numJumps: 1, duration: duration)
                              .SetEase(Ease.Linear)
                              .OnComplete(() => { OnDoneStep(newPosition, targetCell, targetTile); });
        }

        private void ResetTile()
        {
            _isMoving = false;
            Direction = Direction.None;
            UpdateIndicator();
        }

        private void OnDoneStep(Vector2Int newPosition, Cell targetCell, Tile targetTile = null)
        {
            _board.TileMap.Remove(_currentPosition);
            _currentPosition = newPosition;
            if (targetCell.Direction != Direction.None)
            {
                Direction = targetCell.Direction;
                UpdateIndicator();
            }

            _board.TileMap[_currentPosition] = this;
            if (targetTile != null)
            {
                StackTile(targetTile);
                var tileDirection = targetTile.Direction;
                if (tileDirection != Direction.None)
                {
                    _isMoving = false;
                    Direction = tileDirection;
                    UpdateIndicator();
                    targetTile.Direction = Direction.None;
                    targetTile.UpdateIndicator();
                    targetTile._collider.enabled = false;
                    return;
                }
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
            if (Direction == Direction.None) return;
            Move();
        }
    }
}