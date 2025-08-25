using _Projects._SampleAddressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ContentLoader : MonoBehaviour
{
    public void LoadContent(Data data)
    {
        if (data == null)
        {
            Debug.LogError("Data is null, cannot load content!");
            return;
        }

        Debug.Log("Loading prefab from key: " + data.prefabKey);
        Addressables.LoadAssetAsync<GameObject>(data.prefabKey).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Instantiate(handle.Result, Vector3.zero, Quaternion.identity);
                Debug.Log("Prefab instantiated!");
            }
            else
            {
                Debug.LogError("Failed to load prefab: " + data.prefabKey);
            }
        };
    }
}