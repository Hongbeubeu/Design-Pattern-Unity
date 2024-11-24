using System;
using UnityEngine;

[Serializable]
public struct Board
{
    [SerializeField] public int columns;
    [SerializeField] public int rows;
    [SerializeField] public float cellSize;

    public float Width => columns * cellSize;
    public float Height => rows * cellSize;
    public Vector3 Min => new(-Width / 2 + XOffset, 0, -Height / 2 + ZOffset);
    public Vector3 Max => new(Width / 2 - XOffset, 0, Height / 2 - ZOffset);

    private float XOffset => (columns - 1) % 2 * cellSize * 0.5f;
    private float ZOffset => (rows - 1) % 2 * cellSize * 0.5f;

    public Vector3 GridToWorld(int column, int row)
    {
        var x = Min.x + column * cellSize;
        var z = Min.z + row * cellSize;
        return new Vector3(x, 0, z);
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        var x = Mathf.FloorToInt((worldPosition.x - Min.x + XOffset) / cellSize);
        var y = Mathf.FloorToInt((worldPosition.z - Min.z + ZOffset) / cellSize);
        return new Vector2Int(x, y);
    }
}