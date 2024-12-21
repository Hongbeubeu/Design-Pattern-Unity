using System.IO;
using MessagePack;
using UnityEngine;

public class SavedDataController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    private readonly SavedData _savedData = new();

    private void Start()
    {
        LoadDataTest();
    }


    [Button("Save Data")]
    public void SaveDataTest()
    {
        _savedData.Position = _playerTransform.position;
        _savedData.Rotation = _playerTransform.rotation.eulerAngles;
        _savedData.Scale = _playerTransform.localScale;
        SaveData(_savedData);
        
    }

    [Button("Load Data")]
    public void LoadDataTest()
    {
        var data = LoadData();
        _playerTransform.position = data.Position;
        _playerTransform.rotation = Quaternion.Euler(data.Rotation);
        _playerTransform.localScale = data.Scale;
    }

    public void SaveData(SavedData data)
    {
        var serializedData = MessagePackSerializer.Serialize(data);
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "my_saved_data.dat"), serializedData);
    }

    public SavedData LoadData()
    {
        var data = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "my_saved_data.dat"));
        var json = MessagePackSerializer.ConvertToJson(data);
        var savedData = MessagePackSerializer.Deserialize<SavedData>(data);
        return savedData;
    }
}