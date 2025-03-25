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
}