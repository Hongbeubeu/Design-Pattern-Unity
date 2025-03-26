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

    [Serializable]
    public class BoardCell : PoolableMonoBehaviourBase
    {
        [SerializeField] private GameBoard _gameBoard;
        [SerializeField] private GameObject _indicator;
        [SerializeField] private BoardCellData _data;
        public MoveDirection MoveDirection => _data.moveDirection;

        public void UpdateIndicator()
        {
            if (this == null) return;

            if (_gameBoard != null)
            {
                transform.position = _gameBoard.GridToWorldPosition(_data.position);
            }

            if (_indicator == null) return;

            if (MoveDirection == MoveDirection.None)
            {
                _indicator.SetActive(false);

                return;
            }

            _indicator.SetActive(true);
            var directionVector = MoveDirection.GetDirectionVector();
            _indicator.transform.forward = new Vector3(directionVector.x, 0, directionVector.y);
        }

        public void SetupData(GameBoard gameBoard, BoardCellData boardCellData, Transform parent = null)
        {
            _gameBoard = gameBoard;
            _data = boardCellData;
            transform.parent = parent;
            UpdateIndicator();
        }
    }
}