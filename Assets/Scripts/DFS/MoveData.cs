using UnityEngine;

public struct MoveData
{
    public Vector2Int from;
    public Vector2Int to;
    public readonly int numberMoved;

    public MoveData(Vector2Int from, Vector2Int to, int numberMoved)
    {
        this.from = from;
        this.to = to;
        this.numberMoved = numberMoved;
    }
}