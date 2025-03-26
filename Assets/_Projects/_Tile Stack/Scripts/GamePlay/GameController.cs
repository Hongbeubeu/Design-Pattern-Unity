using System;
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
        private void NextLevel()
        {
            if (IsGamePlaying)
                EndGame();
            CurrentLevel++;
            CurrentLevel = Mathf.Clamp(CurrentLevel, 0, DataManager.Instance.MaxLevel - 1);
            StartGame();
        }
    }
}