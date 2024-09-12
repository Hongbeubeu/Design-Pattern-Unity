using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HexagonTest : MonoBehaviour
{
    private readonly List<List<Point>> _listCorners = new();
    private readonly List<Hex> _listHexes = new();
    private Layout _flat;
    private Hex _hex;
    private readonly Hex _hexOrigin = new Hex(100, 100, -200);
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _flat = new Layout(Layout.flat, new Point(1.0f, 1.0f), new Point(0, 0));
        _listCorners.Clear();
        _listHexes.Clear();
        for (var q = -1; q < 2; q++)
        {
            for (var r = -1; r < 2; r++)
            {
                for (var s = -1; s < 2; s++)
                {
                    if (q + r + s != 0) continue;
                    var hex = new Hex(q, r, s);
                    var corners = _flat.PolygonCorners(hex);
                    _listCorners.Add(corners);
                    _listHexes.Add(hex);
                }
            }
        }

        _hex = _hexOrigin;
    }

    private void Update()
    {
        if (!_camera)
        {
            _hex = _hexOrigin;
            return;
        }

        var mousePos = Input.mousePosition;
        var worldPos = _camera.ScreenToWorldPoint(mousePos);
        var point = worldPos.ToPoint();
        _hex = _flat.PixelToHex(point).HexRound();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var corners in _listCorners)
        {
            for (var i = 0; i < corners.Count; i++)
            {
                Gizmos.DrawLine(
                    new Vector3((float)corners[i].x, (float)corners[i].y, 0),
                    new Vector3((float)corners[(i + 1) % corners.Count].x, (float)corners[(i + 1) % corners.Count].y,
                        0));
            }

            for (var i = 0; i < _listHexes.Count; i++)
            {
                var point = _flat.HexToPixel(_listHexes[i]);
                var pos = point.ToVector3();
                if (_hex == _listHexes[i])
                {
                    Gizmos.DrawSphere(pos, 0.1f);
                }

                Handles.Label(pos, $"q: {_listHexes[i].q}, r: {_listHexes[i].r}, s: {_listHexes[i].s}",
                    new GUIStyle
                    {
                        normal = { textColor = Color.black },
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 15
                    });
            }
        }
    }
}