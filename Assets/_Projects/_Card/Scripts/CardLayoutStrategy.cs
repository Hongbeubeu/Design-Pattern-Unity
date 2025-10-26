using UnityEngine;

public struct LayoutData
{
    public Vector3[] openPositions;
    public Quaternion[] openRotations;
    public Vector3[] closedPositions;
    public Quaternion[] closedRotations;

    public LayoutData(int cardCount)
    {
        openPositions = new Vector3[cardCount];
        openRotations = new Quaternion[cardCount];
        closedPositions = new Vector3[cardCount];
        closedRotations = new Quaternion[cardCount];
    }
}

public abstract class CardLayoutStrategy : MonoBehaviour
{
    public abstract LayoutData CalculateLayouts(int cardCount);

    public abstract bool CheckForRuntimeChanges();
}