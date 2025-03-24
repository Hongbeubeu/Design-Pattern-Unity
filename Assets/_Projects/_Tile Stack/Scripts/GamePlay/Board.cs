using System.Collections.Generic;
using ObjectPooler;
using Tool;
using UnityEngine;

namespace TileStack
{
    public class Board : MonoBehaviour
    {
        [SerializeField]
        private LevelDatabase _levelDatabase;

        [SerializeField]
        private Cell _cellPrefab;

        [SerializeField]
        private Tile _tilePrefab;

        [SerializeField]
        private GroundCell _groundCellPrefab;

        [SerializeField]
        private List<Cell> _cells = new();

        [SerializeField]
        private List<Tile> _tiles = new();

        public int Width => _levelDatabase.GetLevel(_currentLevelIndex).width;
        public int Height => _levelDatabase.GetLevel(_currentLevelIndex).height;
        public Dictionary<Vector2Int, Tile> TileMap { get; } = new();
        private Dictionary<Vector2Int, Cell> CellMap { get; } = new();

        private int _currentLevelIndex;

        private void Awake()
        {
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_cellPrefab), _cellPrefab, 25, 25);
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_groundCellPrefab), _groundCellPrefab, 50, 50);
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_tilePrefab), _tilePrefab, 25, 25);
        }

        private void Start()
        {
            CreateBoard();
            MapTiles();
            MapCells();
            CreateGround();
        }

        private void CreateGround()
        {
            for (var x = -Width; x < 2 * Width; x++)
            {
                for (var y = -Height; y < 2 * Height; y++)
                {
                    var position = new Vector2Int(x, y);
                    if (CellMap.ContainsKey(position)) continue;
                    var groundCell = ObjectPoolManager.Instance.GetObject<GroundCell>(_groundCellPrefab);
                    groundCell.transform.position = position.GridToWorldPosition(Width, Height);
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
                CellMap[cell.transform.position.WorldToGridPosition(Width, Height)] = cell;
            }
        }

        [Button]
        private void CreateBoard()
        {
            CreateGround();
            var level = _levelDatabase.GetLevel(_currentLevelIndex);
            if (level.width <= 0 || level.height <= 0)
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

            foreach (var c in level.cells)
            {
                var position = c.position.GridToWorldPosition(level.width, level.height);
                position.y = _cellPrefab.transform.position.y;
                var cell = ObjectPoolManager.Instance.GetObject<Cell>(_cellPrefab);
                cell.transform.position = position;
                cell.transform.parent = transform;
                cell.Board = this;
                cell.CellData = new CellData(c);
                _cells.Add(cell);
                cell.UpdateIndicator();
            }

            foreach (var t in level.tiles)
            {
                var position = t.position.GridToWorldPosition(level.width, level.height);
                var tile = ObjectPoolManager.Instance.GetObject<Tile>(_tilePrefab);
                tile.transform.position = position;
                tile.transform.parent = transform;
                tile.Board = this;
                tile.TileData = new TileData(t);
                _tiles.Add(tile);
                tile.UpdateIndicator();
            }
        }

        public Cell GetCell(Vector2Int position)
        {
            if (!IsInsideBoard(position)) return null;
            var index = position.x * Height + position.y;
            return _cells[index];
        }

        public bool HasCell(Vector2Int position)
        {
            if (!IsInsideBoard(position)) return false;
            return CellMap.ContainsKey(position);
        }

        private bool IsInsideBoard(Vector2Int position)
        {
            return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
        }

        public bool TryGetTileAt(Vector2Int position, out Tile tile)
        {
            tile = null;
            TileMap.TryGetValue(position, out tile);
            return tile != null;
        }
    }
}