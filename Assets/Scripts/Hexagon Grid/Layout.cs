using System;
using System.Collections.Generic;
using UnityEngine;

public struct Layout
{
    public readonly Orientation orientation;
    public readonly Point size;
    public readonly Point origin;

    public static Orientation pointy = new(
        Math.Sqrt(3.0),
        Math.Sqrt(3.0) / 2.0,
        0.0,
        3.0 / 2.0,
        Math.Sqrt(3.0) / 3.0,
        -1.0 / 3.0,
        0.0,
        2.0 / 3.0,
        0.5);

    public static Orientation flat = new(
        3.0 / 2.0,
        0.0,
        Math.Sqrt(3.0) / 2.0,
        Math.Sqrt(3.0),
        2.0 / 3.0,
        0.0,
        -1.0 / 3.0,
        Math.Sqrt(3.0) / 3.0,
        0.0);

    public Layout(Orientation orientation, Point size, Point origin)
    {
        this.orientation = orientation;
        this.size = size;
        this.origin = origin;
    }
    
    public Point HexToPixel(Hex h)
    {
        var m = orientation;
        var x = (m.f0 * h.q + m.f1 * h.r) * size.x;
        var y = (m.f2 * h.q + m.f3 * h.r) * size.y;
        return new Point(x + origin.x, y + origin.y);
    }

    public FractionalHex PixelToHex(Point p)
    {
        var m = orientation;
        var pt = new Point((p.x - origin.x)/ size.x, (p.y - origin.y) / size.y);
        var q = m.b0 * pt.x + m.b1 * pt.y;
        var r = m.b2 * pt.x + m.b3 * pt.y;
        return new FractionalHex(q, r, -q - r);
    }

    public Point HexCornerOffset(int corner)
    {
        var m = orientation;
        var angle = 2.0 * Math.PI * (m.startAngle - corner) / 6;
        return new Point(size.x * Math.Cos(angle), size.y * Math.Sin(angle));
    }
    
    public List<Point> PolygonCorners(Hex h)
    {
        var corners = new List<Point>();
        var center = HexToPixel(h);
        for (var i = 0; i < 6; i++)
        {
            var offset = HexCornerOffset(i);
            corners.Add(new Point(center.x + offset.x, center.y + offset.y));
        }

        return corners;
    }
}