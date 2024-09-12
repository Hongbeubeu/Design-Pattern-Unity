using System;
using System.Collections.Generic;

public readonly struct Hex : IEquatable<Hex>
{
    public readonly int q;

    public readonly int r;

    public readonly int s;

    public Hex(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }

    public bool Equals(Hex other)
    {
        return q == other.q && r == other.r && s == other.s;
    }

    public override bool Equals(object obj)
    {
        return obj is Hex other && Equals(other);
    }

    public static bool operator ==(Hex left, Hex right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(Hex left, Hex right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(q, r, s);
    }

    public Hex Add(Hex b)
    {
        return new Hex(q + b.q, r + b.r, s + b.s);
    }

    public Hex Subtract(Hex b)
    {
        return new Hex(q - b.q, r - b.r, s - b.s);
    }

    public Hex Scale(int k)
    {
        return new Hex(q * k, r * k, s * k);
    }

    public Hex RotateLeft()
    {
        return new Hex(-s, -q, -r);
    }

    public Hex RotateRight()
    {
        return new Hex(-r, -s, -q);
    }

    public static List<Hex> directions = new List<Hex>
                                         {
                                             new Hex(1, 0, -1),
                                             new Hex(1, -1, 0),
                                             new Hex(0, -1, 1),
                                             new Hex(-1, 0, 1),
                                             new Hex(-1, 1, 0),
                                             new Hex(0, 1, -1)
                                         };

    public static Hex Direction(int direction)
    {
        return directions[direction];
    }

    public Hex Neighbor(int direction)
    {
        return Add(Direction(direction));
    }

    public static List<Hex> diagonals = new List<Hex>
                                        {
                                            new Hex(2, -1, -1),
                                            new Hex(1, -2, 1),
                                            new Hex(-1, -1, 2),
                                            new Hex(-2, 1, 1),
                                            new Hex(-1, 2, -1),
                                            new Hex(1, 1, -2)
                                        };

    public Hex DiagonalNeighbor(int direction)
    {
        return Add(diagonals[direction]);
    }

    public int Length()
    {
        return (Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2;
    }

    public int Distance(Hex b)
    {
        return Subtract(b).Length();
    }
}