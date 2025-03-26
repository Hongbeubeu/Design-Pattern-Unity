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
}