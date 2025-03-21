using System.Collections.Generic;
using Tool;
using UnityEngine;

namespace TileStack
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private List<Cell> _cells = new();
        [SerializeField] private List<Tile> _tiles = new();

        private readonly Dictionary<Vector2Int, Tile> _tileMap = new();

        public Dictionary<Vector2Int, Tile> TileMap => _tileMap;

        private void Start()
        {
            MapTiles();
        }

        private void MapTiles()
        {
            foreach (var tile in _tiles)
            {
                _tileMap[tile.CurrentPosition] = tile;
            }
        }

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

            gameObject.DestroyAllChildrenImmediate();
            _cells.Clear();

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var position = new Vector2Int(x, y).GridToWorldPosition(_width, _height);
                    position.y = _cellPrefab.transform.position.y;
                    var cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                    _cells.Add(cell);
                }
            }
        }

        public Cell GetCell(Vector2Int position)
        {
            var index = position.x * _height + position.y;
            return _cells[index];
        }

        public bool IsInsideBoard(Vector2Int position)
        {
            return position.x >= 0 && position.x < _width && position.y >= 0 && position.y < _height;
        }

        public bool TryGetTileAt(Vector2Int position, out Tile tile)
        {
            tile = null;
            _tileMap.TryGetValue(position, out tile);
            return tile != null;
        }
    }
}