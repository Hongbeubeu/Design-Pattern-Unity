using System.Collections;
using UnityEngine;

public enum GridOrientation
{
    PointyTopped,
    FlatTopped
}

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexPrefab; // Hexagon tile prefab
    public int gridWidth = 10; // Number of columns
    public int gridHeight = 10; // Number of rows
    public float hexRadius = 1f; // Radius of the hexagon
    public float spacing = 0.1f; // Spacing between hexagons
    public GridOrientation orientation = GridOrientation.PointyTopped;

    private static readonly float Sqrt3 = Mathf.Sqrt(3); // Store sqrt(3) as a static variable
    private float PointyTopColumnDistance => hexRadius * Sqrt3 + spacing;
    private float PointyTopRowDistance => hexRadius * 1.5f + spacing;
    private float FlatTopColumnDistance => hexRadius * 1.5f + spacing;
    private float FlatTopRowDistance => hexRadius * Sqrt3 + spacing;

    private void Start()
    {
        StartCoroutine(GenerateHexGrid());
    }

    private IEnumerator GenerateHexGrid()
    {
        for (var r = 0; r < gridHeight; r++)
        {
            for (var q = 0; q < gridWidth; q++)
            {
                // Calculate the position for this hex tile
                var hexPosition = CalculateHexPosition(q, r);

                // Instantiate the hex prefab at this position
                var hex = Instantiate(hexPrefab, hexPosition, hexPrefab.transform.rotation);
                hex.transform.parent = transform;
                hex.GetComponent<HexagonGizmo>().Setup(q, r);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private Vector3 CalculateHexPosition(int q, int r)
    {
        return orientation == GridOrientation.PointyTopped
                   ? CalculatePointyTopHexPosition(q, r)
                   : CalculateFlatTopHexPosition(q, r);
    }

    private Vector3 CalculateFlatTopHexPosition(int columnIndex, int rowIndex)
    {
        var x = FlatTopColumnDistance * columnIndex;
        var y = FlatTopRowDistance * (rowIndex + columnIndex / 2.0f);

        return new Vector3(x, y, 0f);
    }

    private Vector3 CalculatePointyTopHexPosition(int columnIndex, int rowIndex)
    {
        var x = PointyTopColumnDistance * (columnIndex + rowIndex / 2.0f);
        var y = PointyTopRowDistance * rowIndex;

        return new Vector3(x, y, 0f);
    }
}