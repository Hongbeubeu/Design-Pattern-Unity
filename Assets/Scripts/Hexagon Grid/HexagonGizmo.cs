using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class HexagonGizmo : MonoBehaviour
{
    [SerializeField] public float hexRadius = 1f; // Radius of the hexagon
    public Color hexColor = Color.green; // Color of the hexagon lines in the editor

    private int q, r;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = hexColor;
        DrawHexagon(transform.position, hexRadius);
        Handles.Label(
            transform.position,
            $"q: {q}, r: {r}",
            new GUIStyle
            {
                normal = { textColor = Color.black },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20
            });
    }

    public void Setup(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    private void DrawHexagon(Vector3 center, float radius)
    {
        // Calculate the positions of the 6 vertices of the hexagon
        var vertices = new Vector3[6];
        for (var i = 0; i < 6; i++)
        {
            float angleDeg = 60 * i; // 60 degrees between each vertex
            var angleRad = Mathf.Deg2Rad * angleDeg;
            var vertex = new Vector3(
                radius * Mathf.Cos(angleRad),
                radius * Mathf.Sin(angleRad),
                0
            );
            
            vertices[i] = center + transform.rotation * vertex;
        }

        // Draw lines between the vertices
        for (var i = 0; i < 6; i++)
        {
            var start = vertices[i];
            var end = vertices[(i + 1) % 6];
            Gizmos.DrawLine(start, end);
        }
    }
}