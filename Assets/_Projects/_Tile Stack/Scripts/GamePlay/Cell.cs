using System;
using UnityEngine;

namespace TileStack
{
    [Serializable]
    public class Cell
    {
        private int _x;
        private int _y;

        public Vector2Int Position => new(_x, _y);
        public Transform Transform { get; }

        public Cell(int x, int y, Transform transform)
        {
            _x = x;
            _y = y;
            Transform = transform;
        }
    }
}