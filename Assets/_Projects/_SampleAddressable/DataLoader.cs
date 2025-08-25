using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using _Projects._SampleAddressable;

public class DataLoader : MonoBehaviour
{
    [SerializeField] private string _dataKey = "MyData"; // key để load Data
    public Action<Data> onDataLoaded; // callback

    public void LoadData()
    {
        Addressables.LoadAssetAsync<Data>(_dataKey).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Data loaded from Addressables: " + _dataKey);
                onDataLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError("Failed to load Data: " + _dataKey);
            }
        };
    }
}