using System.Collections.Generic;
using UnityEngine;

public class ObjectsArrangement : MonoBehaviour
{
    [SerializeField] private List<Transform> _objects = new();

    [Button("GetObjects")]
    private void GetObjects()
    {
        _objects.Clear();
        var count = transform.childCount;
        for (var i = 0; i < count; i++)
        {
            var child = transform.GetChild(i);
            if (child.TryGetMeshFilter(out _))
            {
                _objects.Add(child);
            }
        }
    }

    [Button("ArrangeObjects")]
    private void ArrangeObjects()
    {
        if (_objects.Count == 0)
        {
            Debug.LogWarning("No objects to arrange.");
            return;
        }

        var count = _objects.Count;
        var previousPart = 0f;
        var position = Vector3.zero;
        for (var i = 0; i < count; i++)
        {
            var obj = _objects[i];
            var currentPart = obj.localPosition.x - obj.GetMin().x;
            position.x += previousPart + currentPart;
            obj.localPosition = position;
            previousPart = obj.GetMax().x - obj.localPosition.x;
        }
    }
}