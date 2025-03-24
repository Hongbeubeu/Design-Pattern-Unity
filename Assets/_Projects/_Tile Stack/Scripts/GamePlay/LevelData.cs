using System;
using UnityEngine;

namespace TileStack
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
    public class LevelData : ScriptableObject
    {
        public int width;
        public int height;
        public CellData[] cells;
        public TileData[] tiles;
    }

    [Serializable]
    public class TileData
    {
        public Vector2Int position;
        public Direction direction;

        public TileData(TileData tileData)
        {
            position = tileData.position;
            direction = tileData.direction;
        }
    }

    [Serializable]
    public class CellData
    {
        public Vector2Int position;
        public Direction direction;

        public CellData(CellData cellData)
        {
            position = cellData.position;
            direction = cellData.direction;
        }
    }
}