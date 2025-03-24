using System;
using UnityEngine;

namespace TileStack
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "LevelDatabase")]
    public class LevelDatabase : ScriptableObject
    {
        [SerializeField] private LevelData[] _levels;

        public LevelData GetLevel(int index)
        {
            if (index < 0 || index >= _levels.Length)
            {
                throw new ArgumentException($"Index is out of range. Index: {index}");
            }

            return _levels[index];
        }
    }

    [Serializable]
    public class LevelData
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
    }

    [Serializable]
    public class CellData
    {
        public Vector2Int position;
        public Direction direction;
    }
}