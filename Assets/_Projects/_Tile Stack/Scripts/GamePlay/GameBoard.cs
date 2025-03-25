using System.Collections.Generic;
using ObjectPooler;
using Tool;
using UnityEngine;

namespace TileStack
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private LevelDatabase _levelDatabase;
        [SerializeField] private BoardCell _boardCellPrefab;
        [SerializeField] private StackTile _stackTilePrefab;
        [SerializeField] private DecorationCell _decorationCellPrefab;

        private int Width => _levelDatabase.GetLevel(_currentLevelIndex).width;

        private int Height => _levelDatabase.GetLevel(_currentLevelIndex).height;

        public Dictionary<Vector2Int, StackTile> TileMap { get; } = new();

        private Dictionary<Vector2Int, BoardCell> CellMap { get; } = new();

        private int _currentLevelIndex;

        private void Awake()
        {
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_boardCellPrefab), _boardCellPrefab, 25, 25);
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_decorationCellPrefab), _decorationCellPrefab, 50, 50);
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_stackTilePrefab), _stackTilePrefab, 25, 25);
        }

        private void Start()
        {
            SpawnBoard();
            SpawnDecoration();
        }

        public void RemoveTileMap(Vector2Int position)
        {
            TileMap.Remove(position);
        }

        [Button]
        private void SpawnBoard()
        {
            SpawnDecoration();
            var level = _levelDatabase.GetLevel(_currentLevelIndex);

            if (level.width <= 0 || level.height <= 0)
            {
                Debug.LogWarning("Width and height must be greater than 0.");

                return;
            }

            if (_boardCellPrefab == null)
            {
                Debug.LogWarning("Cell prefab is null.");

                return;
            }

            gameObject.DestroyAllChildrenImmediate();

            foreach (var designedCellData in level.designedCellDatas)
            {
                SpawnBoardCell(designedCellData);

                if (!designedCellData.hasTile) continue;
                SpawnStackTile(designedCellData);
            }
        }

        private void SpawnBoardCell(DesignedCellData designedCellData)
        {
            var cell = ObjectPoolManager.Instance.GetObject<BoardCell>(_boardCellPrefab);
            var direction = designedCellData.hasTile ? MoveDirection.None : designedCellData.moveDirection;
            cell.SetupData(this, new CellData(designedCellData.position, direction), transform);
            CellMap[designedCellData.position] = cell;
        }

        private void SpawnStackTile(DesignedCellData designedCellData)
        {
            var tile = ObjectPoolManager.Instance.GetObject<StackTile>(_stackTilePrefab);
            tile.SetupData(this, new TileData(designedCellData.position, designedCellData.moveDirection), transform);
            TileMap[designedCellData.position] = tile;
        }

        private void SpawnDecoration()
        {
            for (var x = -Width; x < 2 * Width; x++)
            {
                for (var y = -Height; y < 2 * Height; y++)
                {
                    var position = new Vector2Int(x, y);

                    if (CellMap.ContainsKey(position)) continue;
                    var groundCell = ObjectPoolManager.Instance.GetObject<DecorationCell>(_decorationCellPrefab);
                    groundCell.transform.position = GridToWorldPosition(position);
                    groundCell.transform.parent = transform;
                }
            }
        }

        public BoardCell GetCell(Vector2Int position)
        {
            CellMap.TryGetValue(position, out var cell);

            return cell;
        }

        public bool HasCell(Vector2Int position)
        {
            return IsInsideBoard(position) && CellMap.ContainsKey(position);
        }

        private bool IsInsideBoard(Vector2Int position)
        {
            return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
        }

        public bool TryGetTileAt(Vector2Int position, out StackTile stackTile)
        {
            stackTile = null;
            TileMap.TryGetValue(position, out stackTile);

            return stackTile;
        }

        public Vector3 GridToWorldPosition(Vector2Int position)
        {
            var offset = new Vector3((Width - 1) / 2f, 0, (Height - 1) / 2f);

            return new Vector3(position.x, 0, position.y) - offset;
        }

        public Vector2Int WorldToGridPosition(Vector3 position)
        {
            var offset = new Vector3((Width - 1) / 2f, 0, (Height - 1) / 2f);

            return new Vector2Int(Mathf.RoundToInt(position.x + offset.x), Mathf.RoundToInt(position.z + offset.z));
        }

        private void OnDrawGizmos()
        {
            foreach (var tile in TileMap)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(tile.Value.transform.position, Vector3.one);
            }
        }
    }
}