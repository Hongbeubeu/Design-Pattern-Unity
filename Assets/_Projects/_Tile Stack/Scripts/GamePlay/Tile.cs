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
        [SerializeField] private Direction _direction;
        [SerializeField] private Board _board;
        [SerializeField] private Transform _indicator;
        [SerializeField] private Transform _jumpTarget;
        [SerializeField] private Vector2Int _currentPosition;
        [SerializeField] private float _duration = 1f;

        public Vector2Int CurrentPosition => _currentPosition;
        public Transform JumpTarget => _jumpTarget;
        private Vector2Int DirectionVector => _direction.GetDirectionVector();

        private bool _isMoving;

        private void OnValidate()
        {
            UpdateIndicator();
        }

        private void Start()
        {
            UpdateIndicator();
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
            transform.DOMove(targetPosition, duration: _duration)
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
            transform.DOJump(targetPosition, jumpPower: 1f, numJumps: 1, duration: _duration)
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
                targetTile.transform.SetParent(transform, true);
                _board.TileMap.Remove(_currentPosition);
            }

            Move();
        }
    }
}