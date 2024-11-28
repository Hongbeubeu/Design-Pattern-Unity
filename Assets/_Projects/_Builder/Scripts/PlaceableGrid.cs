using System;
using UnityEngine;

[Serializable]
public struct Grid2D
{
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private float _cellSize;
    [SerializeField] private Transform _origin;

    public int Rows => _rows;
    public int Columns => _columns;
    public float CellSize => _cellSize;
    public Transform Origin => _origin;

    public Grid2D(int rows, int columns, float cellSize, Transform origin)
    {
        _rows = rows;
        _columns = columns;
        _cellSize = cellSize;
        _origin = origin;
    }
}

public class PlaceableGrid : MonoBehaviour
{
    [SerializeField] private Grid2D _grid;

    private void OnDrawGizmos()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        if (_grid.Origin == null)
        {
            return;
        }

        // _gridNormal = _grid.Origin.up;

        // Xác định góc quay từ Vector3.up sang vector normal
        var rotation = _grid.Origin.rotation;

        // Tính góc bắt đầu (tâm là gốc của grid)
        var start = -new Vector3(_grid.Columns * _grid.CellSize * 0.5f, 0, _grid.Rows * _grid.CellSize * 0.5f);

        // Vẽ các đường ngang
        for (var i = 0; i <= _grid.Rows; i++)
        {
            var startRow = rotation * (start + new Vector3(0, 0, i * _grid.CellSize));
            var endRow = rotation * (start + new Vector3(_grid.Columns * _grid.CellSize, 0, i * _grid.CellSize));

            Gizmos.DrawLine(_grid.Origin.position + startRow, _grid.Origin.position + endRow);
        }

        // Vẽ các đường dọc
        for (var j = 0; j <= _grid.Columns; j++)
        {
            var startColumn = rotation * (start + new Vector3(j * _grid.CellSize, 0, 0));
            var endColumn = rotation * (start + new Vector3(j * _grid.CellSize, 0, _grid.Rows * _grid.CellSize));

            Gizmos.DrawLine(_grid.Origin.position + startColumn, _grid.Origin.position + endColumn);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_grid.Origin.position, _grid.Origin.position + _grid.Origin.up.normalized);
    }
}