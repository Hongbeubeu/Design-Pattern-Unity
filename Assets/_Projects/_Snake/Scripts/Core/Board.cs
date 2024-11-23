using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct BoardSize
{
    [SerializeField] public int width;
    [SerializeField] public int height;

    public BoardSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}

public struct BoardBounds
{
    public Vector3 min;
    public Vector3 max;

    public BoardBounds(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }
}

public class Board : MonoBehaviour
{
    [SerializeField] private Cake _cakePrefab;
    [SerializeField] private GameObject _boardTile;
    [SerializeField] private BoardSize _size;

    private List<Cake> _cakes = new();
    public BoardSize Size => _size;
    public BoardBounds Bounds { get; private set; }

    public void GenerateBoard()
    {
        _boardTile.transform.localScale = new Vector3(Size.width, 1, Size.height);
        Bounds = new BoardBounds(
            new Vector3(-Size.width / 2f, 0, -Size.height / 2f),
            new Vector3(Size.width / 2f, 0, Size.height / 2f)
        );

        SpawnRandomCake();
    }

    public bool IsInsideBoard(Vector3 position)
    {
        return position.x >= Bounds.min.x
            && position.x <= Bounds.max.x
            && position.z >= Bounds.min.z
            && position.z <= Bounds.max.z;
    }

    public Vector3 GetRandomPosition()
    {
        var x = Random.Range(Bounds.min.x + 0.5f, Bounds.max.x - 1f);
        x = Mathf.Ceil(x) + 0.5f;
        var z = Random.Range(Bounds.min.z + 0.5f, Bounds.max.z - 1f);
        z = Mathf.Ceil(z) + 0.5f;
        return new Vector3(x, 1, z);
    }

    public void SpawnCake(Vector3 position)
    {
        var cake = Instantiate(_cakePrefab, position, Quaternion.identity);
        _cakes.Add(cake);
    }

    public void SpawnRandomCake()
    {
        SpawnCake(GetRandomPosition());
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
}