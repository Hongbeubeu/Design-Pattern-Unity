using UnityEngine;

public interface IGrid<T>
{
    Vector2[] GetAllDirections();
    int Size { get; }

    void SetGridValue(Vector2 position, T value);

    T GetGridValue(Vector2 position);
}