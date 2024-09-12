using System;
using System.Collections.Generic;

public struct FractionalHex
{
    public readonly double q;
    public readonly double r;
    public readonly double s;

    public FractionalHex(double q, double r, double s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
        if (Math.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
    }

    public Hex HexRound()
    {
        var qi = (int)Math.Round(q);
        var ri = (int)Math.Round(r);
        var si = (int)Math.Round(s);

        var qDiff = Math.Abs(qi - q);
        var rDiff = Math.Abs(ri - r);
        var sDiff = Math.Abs(si - s);

        if (qDiff > rDiff && qDiff > sDiff)
        {
            qi = -ri - si;
        }
        else if (rDiff > sDiff)
        {
            ri = -qi - si;
        }
        else
        {
            si = -qi - ri;
        }

        return new Hex(qi, ri, si);
    }
    
    public FractionalHex HexLerp(FractionalHex b, double t)
    {
        return new FractionalHex(q * (1.0 - t) + b.q * t, r * (1.0 - t) + b.r * t, s * (1.0 - t) + b.s * t);
    }
    
    public static List<Hex> HexLinedraw(Hex a, Hex b)
    {
        var n = a.Distance(b);
        var aNudge = new FractionalHex(a.q + 1e-6, a.r + 1e-6, a.s - 2e-6);
        var bNudge = new FractionalHex(b.q + 1e-6, b.r + 1e-6, b.s - 2e-6);
        var results = new List<Hex>();
        var step = 1.0 / Math.Max(n, 1);
        for (var i = 0; i <= n; i++)
        {
            results.Add(aNudge.HexLerp(bNudge, step * i).HexRound());
        }

        return results;
    }
}