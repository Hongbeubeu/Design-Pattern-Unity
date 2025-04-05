using hcore.ObjectPooler;
using hcore.Singleton;
using UnityEngine;

namespace TileStack
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private LevelDatabase _levelDatabase;
        [SerializeField] private BoardCell _boardCellPrefab;
        [SerializeField] private StackTile _stackTilePrefab;
        [SerializeField] private DecorationCell _decorationCellPrefab;
        [SerializeField] private Transform _poolParent;
        public int MaxLevel => _levelDatabase.LevelCount;

        private void Awake()
        {
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_boardCellPrefab, _poolParent), _boardCellPrefab, 25, 10);
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_decorationCellPrefab, _poolParent), _decorationCellPrefab, 25, 25);
            ObjectPoolManager.Instance.CreatePool(() => Instantiate(_stackTilePrefab, _poolParent), _stackTilePrefab, 10, 10);
        }

        public BoardCell GetBoardCell()
        {
            return ObjectPoolManager.Instance.GetObject<BoardCell>(_boardCellPrefab);
        }

        public void ReturnBoardCell(BoardCell boardCell)
        {
            boardCell.transform.SetParent(_poolParent);
            boardCell.ReturnToPool();
        }

        public StackTile GetStackTile()
        {
            return ObjectPoolManager.Instance.GetObject<StackTile>(_stackTilePrefab);
        }

        public void ReturnStackTile(StackTile stackTile)
        {
            stackTile.transform.SetParent(_poolParent);
            stackTile.ReturnToPool();
        }

        public DecorationCell GetDecorationCell()
        {
            return ObjectPoolManager.Instance.GetObject<DecorationCell>(_decorationCellPrefab);
        }

        public void ReturnDecorationCell(DecorationCell decorationCell)
        {
            decorationCell.transform.SetParent(_poolParent);
            decorationCell.ReturnToPool();
        }

        public LevelData GetLevel(int levelIndex)
        {
            return _levelDatabase.GetLevel(levelIndex);
        }
    }
}