using System;
using UnityEngine;

namespace TileStack
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
    public class LevelData : ScriptableObject
    {
        public int width;
        public int height;
        public DesignedCellData[] designedCellDatas;
    }

    [Serializable]
    public struct DesignedCellData
    {
        public Vector2Int position;
        public bool hasTile;
        public MoveDirection moveDirection;
    }

    [Serializable]
    public class TileData
    {
        public Vector2Int position;
        public MoveDirection moveDirection;

        public TileData(TileData tileData)
        {
            position = tileData.position;
            moveDirection = tileData.moveDirection;
        }

        public TileData(Vector2Int position, MoveDirection moveDirection)
        {
            this.position = position;
            this.moveDirection = moveDirection;
        }
    }

    [Serializable]
    public class CellData
    {
        public Vector2Int position;
        public MoveDirection moveDirection;

        public CellData(CellData cellData)
        {
            position = cellData.position;
            moveDirection = cellData.moveDirection;
        }

        public CellData(Vector2Int position, MoveDirection moveDirection)
        {
            this.position = position;
            this.moveDirection = moveDirection;
        }
    }
}