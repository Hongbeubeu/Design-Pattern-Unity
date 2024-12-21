using MessagePack;
using UnityEngine;

[MessagePackObject]
public class SavedData
{
    [Key(0)] public Vector3 Position { get; set; }
    [Key(1)] public Vector3 Rotation { get; set; }
    [Key(2)] public Vector3 Scale { get; set; }


    public SavedData()
    {
    }

    public SavedData(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}