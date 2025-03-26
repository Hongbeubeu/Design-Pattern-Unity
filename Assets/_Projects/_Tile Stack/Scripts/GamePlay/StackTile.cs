using System;
using System.Collections.Generic;
using DG.Tweening;
using hcore.ObjectPooler;
using hcore.Tool;
using UnityEngine;

namespace TileStack
{
    public enum MoveDirection
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

    [Serializable]
    public class StackTileData
    {
        public Vector2Int position;
        public MoveDirection moveDirection;

        public StackTileData(StackTileData stackTileData)
        {
            position = stackTileData.position;
            moveDirection = stackTileData.moveDirection;
        }

        public StackTileData(Vector2Int position, MoveDirection moveDirection)
        {
            this.position = position;
            this.moveDirection = moveDirection;
        }
    }

    public class StackTile : PoolableMonoBehaviourBase
    {
        [SerializeField] private GameBoard _gameBoard;
        [SerializeField] private Transform _indicator;
        [SerializeField] private Transform _tileVisual;
        [SerializeField] private Transform _jumpTarget;
        [SerializeField] private Collider _collider;
        [SerializeField] private Vector2Int _currentPosition;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private StackTileData _data;

        private Vector2Int CurrentPosition
        {
            get => _currentPosition;
            set => _currentPosition = value;
        }

        private Transform JumpTarget => _jumpTarget;

        private MoveDirection MoveDirection
        {
            get => _data.moveDirection;
            set => _data.moveDirection = value;
        }

        private Vector2Int DirectionVector => _data.moveDirection.GetDirectionVector();
        private readonly List<StackTile> _stackedTiles = new();
        private Tween _tween;
        private bool _isMoving;

        public void SetupData(GameBoard gameBoard, StackTileData stackTileData, Transform parent = null)
        {
            CurrentPosition = stackTileData.position;
            _gameBoard = gameBoard;
            _data = stackTileData;
            transform.parent = parent;
            _collider.enabled = _data.moveDirection != MoveDirection.None;
            UpdateIndicator();
            UpdatePosition();
        }

        [Button]
        private void UpdateIndicator()
        {
            if (MoveDirection == MoveDirection.None)
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
            if (_gameBoard == null)
            {
                throw new NullReferenceException("<color=red>Game board is null.</color>");
            }

            _currentPosition = _data.position;
            var cell = _gameBoard.GetCell(_currentPosition);

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
            if (_gameBoard == null)
            {
                throw new NullReferenceException("<color=red>GameBoard is null.</color>");
            }

            var newPosition = _currentPosition + DirectionVector;
            var targetCell = _gameBoard.GetCell(newPosition);

            if (_gameBoard.TryGetTileAt(newPosition, out var targetStackTile))
            {
                DoJump(targetStackTile);
            }
            else
            {
                DoMove(targetCell);
            }
        }

        private void DoMove(BoardCell targetCell)
        {
            if (targetCell == null)
            {
                ResetTile();

                return;
            }

            _isMoving = true;
            var targetPosition = targetCell.transform.position;
            targetPosition.y = transform.position.y;
            var duration = Vector3.Distance(transform.position, targetPosition) / _speed;
            _tween = transform.DOMove(targetPosition, duration: duration)
                              .SetEase(Ease.Linear)
                              .OnComplete(() => { OnMoveDone(targetCell); });
        }

        private void OnMoveDone(BoardCell targetBoardCell)
        {
            _gameBoard.RemoveTileMap(_currentPosition);

            _currentPosition = targetBoardCell.Position;

            if (targetBoardCell.MoveDirection != MoveDirection.None)
            {
                MoveDirection = targetBoardCell.MoveDirection;
                UpdateIndicator();
            }

            _gameBoard.TileMap[_currentPosition] = this;
            Move();
        }

        private void DoJump(StackTile targetStackTile)
        {
            _isMoving = true;
            var targetPosition = targetStackTile.JumpTarget.transform.position;
            var duration = Vector3.Distance(transform.position, targetPosition) / _speed;
            _tween = transform.DOJump(targetPosition, jumpPower: 0.1f, numJumps: 1, duration: duration)
                              .SetEase(Ease.InOutCubic)
                              .OnComplete(() => { OnJumpDone(targetStackTile); });
        }

        private void OnJumpDone(StackTile targetStackTile)
        {
            _gameBoard.RemoveTileMap(_currentPosition);
            _currentPosition = targetStackTile.CurrentPosition;
            _gameBoard.TileMap[_currentPosition] = this;

            DoStackTile(targetStackTile);

            if (ShouldStop(targetStackTile)) return;

            Move();
        }

        private void DoStackTile(StackTile targetStackTile)
        {
            var position = transform.position;
            position.y = 0;
            transform.position = position;
            _stackedTiles.Add(targetStackTile);
            var tileStackedTiles = new List<StackTile>(targetStackTile._stackedTiles);
            targetStackTile._stackedTiles.Clear();

            if (tileStackedTiles is { Count: > 0 })
            {
                targetStackTile.ArrangeStackedTiles();
                _stackedTiles.AddRange(tileStackedTiles);

                foreach (var t in tileStackedTiles)
                {
                    t.transform.parent = transform;
                }
            }

            targetStackTile.transform.SetParent(transform, true);
            targetStackTile.transform.localPosition = Vector3.zero;
            ArrangeStackedTiles();
        }

        private void ArrangeStackedTiles()
        {
            var position = _tileVisual.localPosition;
            position.y = GetStackedHeight(0);
            _tileVisual.localPosition = position;

            for (var i = 0; i < _stackedTiles.Count; i++)
            {
                var height = GetStackedHeight(i + 1);
                _stackedTiles[i].transform.localPosition = new Vector3(0, height, 0);
            }
        }

        /// <summary>
        /// If the target stack tile has a move direction, the current stack tile should stop.
        /// </summary>
        /// <param name="targetStackTile"></param>
        /// <returns></returns>
        private bool ShouldStop(StackTile targetStackTile)
        {
            var direction = targetStackTile.MoveDirection;

            if (direction == MoveDirection.None) return false;

            _isMoving = false;
            MoveDirection = direction;
            targetStackTile.SetStacked();
            UpdateIndicator();

            return true;
        }

        private void SetStacked()
        {
            MoveDirection = MoveDirection.None;
            _collider.enabled = false;
            UpdateIndicator();
        }

        private void ResetTile()
        {
            _isMoving = false;
            MoveDirection = MoveDirection.None;
            UpdateIndicator();
        }

        private float GetStackedHeight(int index)
        {
            return (_stackedTiles.Count - index) * _jumpTarget.localPosition.y;
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        public void ReturnToPoolImmediate()
        {
            foreach (var tile in _stackedTiles)
            {
                tile.ArrangeStackedTiles();
                tile.ReturnToPoolImmediate();
            }

            _stackedTiles.Clear();

            DataManager.Instance.ReturnStackTile(this);
            ArrangeStackedTiles();
        }

        private void OnMouseDown()
        {
            if (_isMoving) return;
            if (MoveDirection == MoveDirection.None) return;
            Move();
        }
    }
}