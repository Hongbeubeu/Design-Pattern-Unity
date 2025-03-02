using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetActiveRecursive(this GameObject gameObject, bool active, bool includeThis = true)
    {
        if (includeThis) gameObject.SetActive(active);

        foreach (var child in gameObject.GetComponentsInChildren<Transform>())
        {
            child.gameObject.SetActive(active);
        }
    }

    public static void SetLayerRecursive(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetLayerRecursive(layer);
        }
    }

    public static GameObject FindChildByName(this GameObject gameObject, string name, bool includeThis = true)
    {
        foreach (var child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (!includeThis && child.gameObject == gameObject) continue;
            if (child.name == name)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    public static void DestroyAllChildren(this GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            Object.Destroy(child.gameObject);
        }
    }
    
    public static void DestroyAllChildrenImmediate(this GameObject gameObject)
    {
        foreach (Transform child in gameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }
    
    public static void CopyTransform(this GameObject gameObject, GameObject target)
    {
        gameObject.transform.position = target.transform.position;
        gameObject.transform.rotation = target.transform.rotation;
        gameObject.transform.localScale = target.transform.localScale;
    }

    public static bool TryGetComponent<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.GetComponent<T>();
        return component != null;
    }
    
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject.TryGetComponent(out T component))
        {
            return component;
        }

        return gameObject.AddComponent<T>();
    }
    
    public static void DestroyAllComponents<T>(this GameObject gameObject) where T : Component
    {
        foreach (var component in gameObject.GetComponents<T>())
        {
            Object.Destroy(component);
        }
    }
}
