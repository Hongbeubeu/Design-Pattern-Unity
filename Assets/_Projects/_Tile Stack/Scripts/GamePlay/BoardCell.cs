using System;
using hcore.ObjectPooler;
using UnityEngine;

namespace TileStack
{
    [Serializable]
    public class BoardCellData
    {
        public Vector2Int position;
        public MoveDirection moveDirection;

        public BoardCellData(BoardCellData boardCellData)
        {
            position = boardCellData.position;
            moveDirection = boardCellData.moveDirection;
        }

        public BoardCellData(Vector2Int position, MoveDirection moveDirection)
        {
            this.position = position;
            this.moveDirection = moveDirection;
        }
    }

    public class BoardCell : PoolableMonoBehaviourBase
    {
        [SerializeField] private Indicator _indicator;
        [SerializeField] private BoardCellData _data;

        public MoveDirection MoveDirection => _data.moveDirection;

        public Vector2Int Position => _data.position;

        public void SetupData(BoardCellData boardCellData, Transform parent = null)
        {
            _data = boardCellData;
            transform.parent = parent;
            UpdatePosition();
            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            if (_indicator == null)
            {
                throw new NullReferenceException("<color=red>Indicator is null.</color>");
            }

            _indicator.UpdateIndicator(MoveDirection);
        }

        private void UpdatePosition()
        {
            transform.position = GameController.Instance.GameBoard.GridToWorldPosition(_data.position);
        }
    }
}