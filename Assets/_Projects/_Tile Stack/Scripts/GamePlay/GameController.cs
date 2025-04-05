using hcore.Singleton;
using hcore.Tool;
using TMPro;
using UnityEngine;

namespace TileStack
{
    public class GameController : Singleton<GameController>
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private GameBoard _gameBoard;

        public GameBoard GameBoard => _gameBoard;

        private int CurrentLevel { get; set; }

        private bool IsGamePlaying { get; set; }

        public bool IsAnyTileMoving()
        {
            foreach (var (_, tile) in _gameBoard.TileMap)
            {
                if (tile.Movement.IsMoving) return true;
            }

            return false;
        }

        private void Start()
        {
            StartGame();
        }

        [Button]
        private void StartGame()
        {
            if (IsGamePlaying) return;
            IsGamePlaying = true;
            UpdateLevelText();
            _gameBoard.SetupLevel(CurrentLevel);
            _gameBoard.CreateBoard();
        }

        [Button]
        private void EndGame()
        {
            IsGamePlaying = false;
            _gameBoard.ClearBoard();
        }

        [Button]
        public void RestartGame()
        {
            EndGame();
            StartGame();
        }

        [Button]
        public void NextLevel()
        {
            if (IsGamePlaying) EndGame();
            CurrentLevel++;
            CurrentLevel %= DataManager.Instance.MaxLevel;
            StartGame();
        }

        private void UpdateLevelText()
        {
            _levelText.text = $"Level {CurrentLevel + 1}";
        }
    }
}