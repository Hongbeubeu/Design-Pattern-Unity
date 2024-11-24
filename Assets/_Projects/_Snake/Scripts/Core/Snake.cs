using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private GameObject _snakeTile;
    [SerializeField] private Vector3 _startPosition;

    private Vector3 _direction = Vector3.forward;
    private Vector3 _nextDirection = Vector3.forward;
    private BoardController _boardController;
    private float _timer;
    private bool _canMove = true;
    private readonly List<GameObject> _snakeTiles = new();
    private Vector3 _firstPosition;
    private readonly List<Vector3> _appendPositions = new();

    public List<Vector3> SnakeTilePositions => _snakeTiles.ConvertAll(tile => tile.transform.position);

    public void Initialize(BoardController boardController)
    {
        _boardController = boardController;
        var tile = Instantiate(_snakeTile, _startPosition, Quaternion.identity, transform);
        _snakeTiles.Add(tile);
        _firstPosition = _startPosition;
    }

    public void SetDirection(Vector3 direction)
    {
        if (_direction == -direction)
        {
            return;
        }

        _nextDirection = direction;
    }

    private void Update()
    {
        if (!_canMove) return;
        if (_timer <= 0)
        {
            Move();
            _timer = 1f / _speed;
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }

    private void Move()
    {
        var nextPosition = _firstPosition + _nextDirection;
        if (!Check(nextPosition)) return;

        var lastPosition = _snakeTiles[^1].transform.position;
        var shouldAppend = false;
        if (_appendPositions.Count > 0)
        {
            if (_appendPositions[0] == _snakeTiles[^1].transform.position)
            {
                shouldAppend = true;
            }
        }

        for (var i = _snakeTiles.Count - 1; i >= 0; i--)
        {
            _snakeTiles[i].transform.position = i == 0 ? nextPosition : _snakeTiles[i - 1].transform.position;
        }

        if (shouldAppend)
        {
            var tile = Instantiate(_snakeTile, lastPosition, Quaternion.identity, transform);
            _snakeTiles.Add(tile);
            _appendPositions.RemoveAt(0);
        }

        _firstPosition = nextPosition;
        _direction = _nextDirection;
    }

    private bool Check(Vector3 nextPosition)
    {
        if (!IsEatSelf(nextPosition) && _boardController.IsMovablePosition(nextPosition))
        {
            if (!_boardController.IsCake(nextPosition, out var cake)) return true;

            _boardController.RemoveCake(cake);
            cake.Eat();
            if (_boardController.HasAvailablePosition())
            {
                _boardController.SpawnRandomCake();
            }
            else
            {
                Debug.Log("Win");
                _canMove = false;
            }

            _appendPositions.Add(nextPosition);
            return true;
        }

        Debug.Log("Lose");
        _canMove = false;
        return false;
    }

    private bool IsEatSelf(Vector3 nextPosition)
    {
        foreach (var position in SnakeTilePositions)
        {
            if (position == nextPosition)
            {
                return true;
            }
        }

        return false;
    }
}