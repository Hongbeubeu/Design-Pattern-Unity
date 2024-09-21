using UnityEngine;

public struct MoveData
{
    public Vector2 from;
    public Vector2 to;
    public readonly int numberMoved;

    public MoveData(Vector2 from, Vector2 to, int numberMoved)
    {
        this.from = from;
        this.to = to;
        this.numberMoved = numberMoved;
    }
}