using UnityEngine;

public class GenericSquareGrid<T> : IGrid<T>
{
    private readonly T[,] _grid;
    private readonly Vector2[] _directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    public int Size { get; }

    public GenericSquareGrid(int size)
    {
        Size = size;
        _grid = new T[size, size];
    }

    public Vector2[] GetAllDirections()
    {
        return _directions;
    }

    public void SetGridValue(Vector2 position, T value)
    {
        _grid[(int)position.x, (int)position.y] = value;
    }

    public T GetGridValue(Vector2 position)
    {
        return _grid[(int)position.x, (int)position.y];
    }
}