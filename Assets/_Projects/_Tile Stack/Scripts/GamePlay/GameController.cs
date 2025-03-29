using hcore.Singleton;
using hcore.Tool;
using UnityEngine;

namespace TileStack
{
    public class GameController : Singleton<GameController>
    {
        [SerializeField] private GameBoard _gameBoard;
        private int CurrentLevel { get; set; }
        private bool IsGamePlaying { get; set; }

        private void Start()
        {
            StartGame();
        }

        [Button]
        private void StartGame()
        {
            if (IsGamePlaying) return;
            IsGamePlaying = true;
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
            if (IsGamePlaying)
                EndGame();
            CurrentLevel++;
            CurrentLevel %= DataManager.Instance.MaxLevel;
            StartGame();
        }
    }
}