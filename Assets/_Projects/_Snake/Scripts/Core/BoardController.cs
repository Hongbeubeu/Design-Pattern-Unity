using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] private Cake _cakePrefab;
    [SerializeField] private GameObject _boardTile;
    [SerializeField] private Board _currentBoard;

    public Board CurrentBoard => _currentBoard;
    private readonly List<Cake> _cakes = new();
    private Snake _snake;

    public void GenerateBoard()
    {
        _boardTile.transform.localScale = new Vector3(CurrentBoard.Width, 1, CurrentBoard.Height);
        SpawnRandomCake();
    }

    public void Initialize(Snake snake)
    {
        _snake = snake;
    }

    public bool IsMovablePosition(Vector3 position)
    {
        return position.x >= CurrentBoard.Min.x
            && position.x <= CurrentBoard.Max.x
            && position.z >= CurrentBoard.Min.z
            && position.z <= CurrentBoard.Max.z;
    }

    public Vector3 GetRandomPosition()
    {
        var column = Random.Range(0, CurrentBoard.columns);
        var row = Random.Range(0, CurrentBoard.rows);
        var position = CurrentBoard.GridToWorld(column, row);
        position.y = 1;
        return position;
    }

    public void SpawnCake(Vector3 position)
    {
        var cake = Instantiate(_cakePrefab, position, Quaternion.identity);
        _cakes.Add(cake);
    }

    public void SpawnRandomCake()
    {
        var position = GetRandomPosition();
        if (IsOverlapSnake(position))
        {
            while (IsOverlapSnake(position))
            {
                position = GetRandomPosition();
            }
        }

        SpawnCake(position);
    }

    private bool IsOverlapSnake(Vector3 position)
    {
        if (_snake.SnakeTilePositions == null || _snake.SnakeTilePositions.Count == 0) return false;
        foreach (var pos in _snake.SnakeTilePositions)
        {
            if (pos == position)
                return true;
        }

        return false;
    }

    public bool IsCake(Vector3 position, out Cake cake)
    {
        foreach (var c in _cakes)
        {
            if (c.transform.position != position) continue;
            cake = c;
            return true;
        }

        cake = null;
        return false;
    }

    public void RemoveCake(Cake cake)
    {
        _cakes.Remove(cake);
    }

    public bool HasAvailablePosition()
    {
        return _currentBoard.columns * _currentBoard.rows > _cakes.Count + _snake.SnakeTilePositions.Count;
    }
}