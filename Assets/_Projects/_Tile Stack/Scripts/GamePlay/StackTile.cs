using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using hcore.ObjectPooler;
using hcore.Tool;
using UnityEngine;

namespace TileStack
{
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

    [RequireComponent(typeof(TileMovement))]
    public class StackTile : PoolableMonoBehaviourBase
    {
        [SerializeField] private Indicator _indicator;
        [SerializeField] private TileMovement _movement;
        [SerializeField] private Transform _tileVisual;
        [SerializeField] private Transform _jumpTarget;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private float _singleTileHeight;
        [SerializeField] private Vector2Int _currentPosition;
        [SerializeField] private StackTileData _data;
        public Action onMoveDone;
        private readonly List<StackTile> _stackedTiles = new();

        public Vector2Int CurrentPosition
        {
            get => _currentPosition;
            set => _currentPosition = value;
        }

        public Transform JumpTarget => _jumpTarget;

        public TileMovement Movement => _movement;
        
        public MoveDirection MoveDirection
        {
            get => _data.moveDirection;
            set => _data.moveDirection = value;
        }

        private Vector2Int DirectionVector => _data.moveDirection.GetDirectionVector();

        private GameBoard GameBoard => GameController.Instance.GameBoard;

        public void SetupData(StackTileData stackTileData, Transform parent = null)
        {
            CurrentPosition = stackTileData.position;
            _data = stackTileData;
            transform.parent = parent;
            _collider.enabled = _data.moveDirection != MoveDirection.None;
            _tileVisual.localScale = Vector3.one;
            UpdateIndicator();
            UpdatePosition();
            UpdateCollider();
            onMoveDone += GameBoard.OnTileMoveDone;
            onMoveDone += DoAnimationStacked;
        }

        public void UpdateIndicator()
        {
            if (_indicator == null)
            {
                throw new NullReferenceException("<color=red>Indicator is null.</color>");
            }

            _indicator.UpdateIndicator(MoveDirection);
        }

        private void UpdatePosition()
        {
            _currentPosition = _data.position;
            var cell = GameBoard.GetCell(_currentPosition);

            if (cell == null) return;
            transform.position = cell.transform.position;
        }

        [Button]
        public void ForceMove()
        {
            if (Movement.IsMoving) return;
            Move();
        }

        public void Move()
        {
            var newPosition = _currentPosition + DirectionVector;
            var targetCell = GameBoard.GetCell(newPosition);

            if (GameBoard.TryGetTileAt(newPosition, out var targetStackTile))
            {
                _movement.Setup(new JumpMove(this, targetStackTile));
            }
            else
            {
                _movement.Setup(new LinearMove(this, targetCell));
            }

            _movement.MoveTo(newPosition);
        }

        public void DoStackTile(StackTile targetStackTile)
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
            UpdateCollider();
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

        private void UpdateCollider()
        {
            var size = _collider.size;
            var center = _collider.center;
            size.y = _singleTileHeight * (_stackedTiles.Count + 1);
            center.y = size.y / 2;
            _collider.size = size;
            _collider.center = center;
        }

        public void SetStacked()
        {
            MoveDirection = MoveDirection.None;
            _collider.enabled = false;
            UpdateCollider();
            UpdateIndicator();
        }

        public void ResetTile()
        {
            Movement.IsMoving = false;
            MoveDirection = MoveDirection.None;
            UpdateIndicator();
        }

        private float GetStackedHeight(int index)
        {
            return (_stackedTiles.Count - index) * _jumpTarget.localPosition.y;
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

        private void DoAnimationStacked()
        {
            if (_stackedTiles?.Count == 0) return;
            StartCoroutine(AnimationStackedCoroutine());
        }

        private IEnumerator AnimationStackedCoroutine()
        {
            for (var i = _stackedTiles.Count - 1; i >= 0; i--)
            {
                var tile = _stackedTiles[i];
                tile.DoScale();

                yield return new WaitForSeconds(0.05f);
            }

            DoScale();
        }

        private void DoScale()
        {
            _tileVisual.DOScaleX(1.2f, 0.1f).OnComplete(() => { _tileVisual.DOScaleX(1f, 0.1f); });
            _tileVisual.DOScaleZ(1.2f, 0.1f).OnComplete(() => { _tileVisual.DOScaleZ(1f, 0.1f); });
        }

        public void RemoveFromBoard()
        {
            GameBoard.RemoveTileMap(CurrentPosition);
        }

        public void UpdatePositionOnBoard()
        {
            GameBoard.TileMap[CurrentPosition] = this;
        }

        private void OnDisable()
        {
            transform.DOKill();
            _tileVisual.DOKill();

            if (onMoveDone == null) return;
            onMoveDone -= GameBoard.OnTileMoveDone;
            onMoveDone -= DoAnimationStacked;
            onMoveDone = null;
        }
    }
}