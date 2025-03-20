using System.Collections.Generic;
using Tool;
using UnityEditor;
using UnityEngine;

namespace TileStack
{
    public class Board : MonoBehaviour
    {
        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        [SerializeField]
        private GameObject _cellPrefab;

        [SerializeField]
        private List<GameObject> _cells;

        [Button]
        public void CreateBoard()
        {
            if (_width <= 0 || _height <= 0)
            {
                Debug.LogWarning("Width and height must be greater than 0.");
                return;
            }

            if (_cellPrefab == null)
            {
                Debug.LogWarning("Cell prefab is null.");
                return;
            }

            if (_cells.Count > 0)
            {
                foreach (var cell in _cells)
                {
                    if (cell == null) continue;
                    DestroyImmediate(cell);
                }

                _cells.Clear();
            }

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var cell = Instantiate(_cellPrefab, new Vector3(x, _cellPrefab.transform.position.y, y), Quaternion.identity, transform);
                    _cells.Add(cell);
                }
            }
        }

        public GameObject GetCell(Vector2Int position)
        {
            var index = position.x * _height + position.y;
            return _cells[index];
        }

        public bool IsInsideBoard(Vector2Int position)
        {
            return position.x >= 0 && position.x < _width && position.y >= 0 && position.y < _height;
        }
    }
}