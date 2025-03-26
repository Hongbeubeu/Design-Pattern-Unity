using System.IO;
using hcore.IoC.Services;
using MessagePack;
using UnityEngine;

public sealed class SerializerService : BaseService
{
    protected override void InitializeService()
    {
    }

    protected override void CleanupService()
    {
    }

    public void SerializeSaveData<T>(T obj) where T : class, ISaveData
    {
        var serializedData = MessagePackSerializer.Serialize(obj);
        var savePath = Path.Combine(Application.persistentDataPath, $"{obj.Id}.dat");
        File.WriteAllBytes(savePath, serializedData);
    }

    public bool DeserializeSaveData<T>(string id, out T result) where T : class, ISaveData
    {
        var savePath = Path.Combine(Application.persistentDataPath, $"{id}.dat");
        if (!File.Exists(savePath))
        {
            result = null;
            return false;
        }

        var data = File.ReadAllBytes(savePath);
        result = MessagePackSerializer.Deserialize<T>(data);
        return true;
    }
}