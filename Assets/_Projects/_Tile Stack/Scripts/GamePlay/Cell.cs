using System;
using ObjectPooler;
using UnityEngine;

namespace TileStack
{
    [Serializable]
    public class Cell : PoolableMonoBehaviourBase
    {
        [SerializeField] private Board _board;
        [SerializeField] private GameObject _indicator;
        [SerializeField] private CellData _cellData;

        public Board Board
        {
            get => _board;
            set => _board = value;
        }

        public CellData CellData
        {
            get => _cellData;
            set => _cellData = value;
        }

        public Direction Direction => _cellData.direction;

        private void OnValidate()
        {
#if UNITY_EDITOR
            UpdateIndicator();
#endif
        }

        public void UpdateIndicator()
        {
            if (this == null) return;

            if (_board != null)
            {
                transform.position = _cellData.position.GridToWorldPosition(_board.Width, _board.Height);
            }

            if (_indicator == null) return;
            if (Direction == Direction.None)
            {
                _indicator.SetActive(false);
                return;
            }

            _indicator.SetActive(true);
            var directionVector = Direction.GetDirectionVector();
            _indicator.transform.forward = new Vector3(directionVector.x, 0, directionVector.y);
        }
    }
}