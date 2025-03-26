using System;
using hcore.ObjectPooler;
using UnityEngine;

namespace TileStack
{
    [Serializable]
    public class BoardCell : PoolableMonoBehaviourBase
    {
        [SerializeField] private GameBoard _gameBoard;
        [SerializeField] private GameObject _indicator;
        [SerializeField] private CellData _cellData;

        public MoveDirection MoveDirection => _cellData.moveDirection;

        public void UpdateIndicator()
        {
            if (this == null) return;

            if (_gameBoard != null)
            {
                transform.position = _gameBoard.GridToWorldPosition(_cellData.position);
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

        public void SetupData(GameBoard gameBoard, CellData cellData, Transform parent = null)
        {
            _gameBoard = gameBoard;
            _cellData = cellData;
            transform.parent = parent;
            UpdateIndicator();
        }
    }
}