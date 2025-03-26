using System.Collections.Generic;
using hcore.Tool;
using UnityEngine;

namespace TileStack
{
    public class GameBoard : MonoBehaviour
    {
        private int Width => DataManager.Instance.GetLevel(_currentLevelIndex).width;
        private int Height => DataManager.Instance.GetLevel(_currentLevelIndex).height;
        public Dictionary<Vector2Int, StackTile> TileMap { get; } = new();
        private Dictionary<Vector2Int, BoardCell> CellMap { get; } = new();
        private List<DecorationCell> DecorationCells { get; } = new();
        private int _currentLevelIndex;

        public void SetupLevel(int currentLevel)
        {
            _currentLevelIndex = currentLevel;
        }

        public void CreateBoard()
        {
            SpawnBoard();
            SpawnDecoration();
        }

        public void ClearBoard()
        {
            foreach (var (_, stackTile) in TileMap)
            {
                stackTile.ReturnToPoolImmediate();
            }

            TileMap.Clear();

            foreach (var (_, boardCell) in CellMap)
            {
                DataManager.Instance.ReturnBoardCell(boardCell);
            }

            CellMap.Clear();

            foreach (var decorationCell in DecorationCells)
            {
                DataManager.Instance.ReturnDecorationCell(decorationCell);
            }

            DecorationCells.Clear();
        }

        public void RemoveTileMap(Vector2Int position)
        {
            TileMap.Remove(position);
        }

        [Button]
        private void SpawnBoard()
        {
            var level = DataManager.Instance.GetLevel(_currentLevelIndex);

            foreach (var designedCellData in level.designedCellDatas)
            {
                SpawnBoardCell(designedCellData);

                if (!designedCellData.hasTile) continue;
                SpawnStackTile(designedCellData);
            }
        }

        private void SpawnBoardCell(DesignedCellData designedCellData)
        {
            var cell = DataManager.Instance.GetBoardCell();
            var direction = designedCellData.hasTile ? MoveDirection.None : designedCellData.moveDirection;
            cell.SetupData(this, new BoardCellData(designedCellData.position, direction), transform);
            CellMap[designedCellData.position] = cell;
        }

        private void SpawnStackTile(DesignedCellData designedCellData)
        {
            var tile = DataManager.Instance.GetStackTile();
            tile.SetupData(this, new StackTileData(designedCellData.position, designedCellData.moveDirection), transform);
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
                    var decorationCell = DataManager.Instance.GetDecorationCell();
                    decorationCell.transform.position = GridToWorldPosition(position);
                    decorationCell.transform.parent = transform;
                    DecorationCells.Add(decorationCell);
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