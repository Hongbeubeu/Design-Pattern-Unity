using System.Collections.Generic;
using ObjectPooler;
using Tool;
using UnityEngine;

namespace TileStack
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private LevelDatabase _levelDatabase;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private GroundCell _groundCellPrefab;
        [SerializeField] private List<Cell> _cells = new();
        [SerializeField] private List<Tile> _tiles = new();

        public int Width => _width;
        public int Height => _height;
        public Dictionary<Vector2Int, Tile> TileMap { get; } = new();
        private Dictionary<Vector2Int, Cell> CellMap { get; } = new();

        private int _currentLevelIndex;

        private void Awake()
        {
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_cellPrefab, transform), _cellPrefab, 25, 25);
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_groundCellPrefab, transform), _groundCellPrefab, 50, 50);
        }

        private void Start()
        {
            MapTiles();
            MapCells();
            CreateGround();
        }

        [Button]
        private void CreateGround()
        {
            for (var x = -_width; x < 2 * _width; x++)
            {
                for (var y = -_height; y < 2 * _height; y++)
                {
                    var position = new Vector2Int(x, y);
                    if (CellMap.ContainsKey(position)) continue;
                    var groundCell = ObjectPoolManager.Instance.GetObject<GroundCell>(_groundCellPrefab);
                    groundCell.transform.position = position.GridToWorldPosition(_width, _height);
                    groundCell.transform.parent = transform;
                }
            }
        }

        private void MapTiles()
        {
            foreach (var tile in _tiles)
            {
                TileMap[tile.CurrentPosition] = tile;
            }
        }

        private void MapCells()
        {
            foreach (var cell in _cells)
            {
                CellMap[cell.transform.position.WorldToGridPosition(_width, _height)] = cell;
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
                    cell.Board = this;
                    cell.CellData = new CellData() { position = new Vector2Int(x, y), direction = Direction.None };
                    cell.UpdateIndicator();
                    _cells.Add(cell);
                }
            }
        }

        public Cell GetCell(Vector2Int position)
        {
            if (!IsInsideBoard(position)) return null;
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
            TileMap.TryGetValue(position, out tile);
            return tile != null;
        }
    }
}