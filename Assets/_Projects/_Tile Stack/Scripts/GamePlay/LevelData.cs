using System;
using System.Collections.Generic;
using UnityEngine;

namespace TileStack
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
    public class LevelData : ScriptableObject
    {
        public int width;
        public int height;
        public List<DesignedCellData> designedCellDatas = new();
    }

    [Serializable]
    public class DesignedCellData
    {
        public Vector2Int position;
        public bool hasTile;
        public MoveDirection moveDirection;
    }
}