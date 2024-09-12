using UnityEngine;

public static class HexagonExtensions
{
    public static Vector3 ToVector3(this Point point)
    {
        return new Vector3((float)point.x, (float)point.y, 0);
    }

    public static Vector2 ToVector2(this Point point)
    {
        return new Vector2((float)point.x, (float)point.y);
    }
    
    public static Point ToPoint(this Vector3 vector)
    {
        return new Point(vector.x, vector.y);
    }
    
    public static Point ToPoint(this Vector2 vector)
    {
        return new Point(vector.x, vector.y);
    }
}