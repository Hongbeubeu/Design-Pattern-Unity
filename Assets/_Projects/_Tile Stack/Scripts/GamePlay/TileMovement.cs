using System;
using DG.Tweening;
using hcore.Tool;
using UnityEngine;

namespace TileStack
{
    public interface ITileMoveStrategy
    {
        Action OnBeginMove { get; set; }

        Action OnEndMove { get; set; }

        StackTile MoveTile { get; set; }

        void MoveTo(Vector2Int targetGridPosition);
    }

    public class LinearMove : ITileMoveStrategy
    {
        public Action OnBeginMove { get; set; }

        public Action OnEndMove { get; set; }

        public StackTile MoveTile { get; set; }

        private readonly BoardCell _targetCell;

        public LinearMove(StackTile moveTile, BoardCell targetCell)
        {
            MoveTile = moveTile;
            _targetCell = targetCell;
        }

        public void MoveTo(Vector2Int targetGridPosition)
        {
            OnBeginMove?.Invoke();

            if (_targetCell == null)
            {
                MoveTile.ResetTile();
                MoveTile.onMoveDone?.Invoke();

                return;
            }

            MoveTile.Movement.IsMoving = true;
            var targetPosition = _targetCell.transform.position;
            targetPosition.y = MoveTile.transform.position.y;
            var duration = Vector3.Distance(MoveTile.transform.position, targetPosition) / MoveTile.Movement.Speed;
            MoveTile.transform.DOMove(targetPosition, duration: duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { OnMoveDone(_targetCell); });
        }

        private void OnMoveDone(BoardCell targetBoardCell)
        {
            MoveTile.RemoveFromBoard();
            MoveTile.CurrentPosition = targetBoardCell.Position;

            if (targetBoardCell.MoveDirection != MoveDirection.None)
            {
                MoveTile.MoveDirection = targetBoardCell.MoveDirection;
                MoveTile.UpdateIndicator();
            }

            MoveTile.UpdatePositionOnBoard();
            OnEndMove?.Invoke();
            MoveTile.Move();
        }
    }

    public class JumpMove : ITileMoveStrategy
    {
        public Action OnBeginMove { get; set; }

        public Action OnEndMove { get; set; }

        public StackTile MoveTile { get; set; }

        private readonly StackTile _targetTile;

        public JumpMove(StackTile moveTile, StackTile targetTile)
        {
            MoveTile = moveTile;
            _targetTile = targetTile;
        }

        public void MoveTo(Vector2Int targetGridPosition)
        {
            OnBeginMove?.Invoke();

            MoveTile.Movement.IsMoving = true;
            var targetPosition = _targetTile.JumpTarget.transform.position;
            var sequence = DOTween.Sequence();
            var startPos = MoveTile.transform.position;
            sequence.Append(MoveTile.transform.DOMoveY(startPos.y - 0.2f, 0.1f).SetEase(Ease.InOutBack));
            sequence.Append(MoveTile.transform.DOJump(targetPosition, jumpPower: 0.2f, numJumps: 1, duration: 0.2f));
            sequence.OnComplete(() => { OnJumpDone(_targetTile); });
        }

        private void OnJumpDone(StackTile targetStackTile)
        {
            MoveTile.RemoveFromBoard();
            MoveTile.CurrentPosition = targetStackTile.CurrentPosition;
            MoveTile.UpdatePositionOnBoard();
            MoveTile.DoStackTile(targetStackTile);

            if (ShouldStop(targetStackTile))
            {
                MoveTile.onMoveDone?.Invoke();
                MoveTile.Movement.IsMoving = false;
                MoveTile.MoveDirection = targetStackTile.MoveDirection;
                targetStackTile.SetStacked();
                MoveTile.UpdateIndicator();

                return;
            }

            OnEndMove?.Invoke();
            MoveTile.Move();
        }

        /// <summary>
        /// If the target stack tile has a move direction, the current stack tile should stop.
        /// </summary>
        /// <param name="targetStackTile"></param>
        /// <returns></returns>
        private bool ShouldStop(StackTile targetStackTile)
        {
            var direction = targetStackTile.MoveDirection;

            return direction != MoveDirection.None;
        }
    }

    public class QuantumMove : ITileMoveStrategy
    {
        public Action OnBeginMove { get; set; }

        public Action OnEndMove { get; set; }

        public StackTile MoveTile { get; set; }

        public void MoveTo(Vector2Int targetGridPosition)
        {
        }
    }

    public class TileMovement : MonoBehaviour
    {
        [SerializeField] private float _speed;

        public bool IsMoving { get; set; }

        public float Speed => _speed;

        private ITileMoveStrategy _tileMoveStrategy;

        public void Setup(ITileMoveStrategy tileMoveStrategy)
        {
            _tileMoveStrategy = tileMoveStrategy;
        }

        public void MoveTo(Vector2Int targetPosition)
        {
            _tileMoveStrategy.MoveTo(targetPosition);
        }

        [Button]
        public void ForceMove()
        {
            if (IsMoving) return;
        }
    }
}