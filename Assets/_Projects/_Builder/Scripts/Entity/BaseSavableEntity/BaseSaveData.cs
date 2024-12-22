using System.IO;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public abstract class BaseSaveData : ISaveData
{
    [Key(0)]
    public string Id { get; }

    protected string SavePath { get; }

    protected BaseSaveData()
    {
    }

    protected BaseSaveData(string id)
    {
        Id = id;
        SavePath = Path.Combine(Application.persistentDataPath, $"{Id}.dat");
    }

    public abstract void Serialize();

    public abstract void Deserialize();
    protected bool CanDeserialize()
    {
        return File.Exists(SavePath);
    }
}