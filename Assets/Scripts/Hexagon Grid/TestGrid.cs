using UnityEngine;

public class TestGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;
    private Vector3Int _cellPosition;

    private void Start()
    {
        grid ??= GetComponent<Grid>();
    }

    private void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _cellPosition = grid.WorldToCell(mousePosition);
    }

    private void OnDrawGizmos()
    {
        if (grid == null) return;
        var cellCenterPosition = grid.CellToWorld(_cellPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cellCenterPosition, 0.1f);
    }
}