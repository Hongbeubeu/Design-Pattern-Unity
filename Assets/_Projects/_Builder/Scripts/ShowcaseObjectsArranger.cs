using System.Collections.Generic;
using UnityEngine;

public class ShowcaseObjectsArranger : MonoBehaviour
{
    [SerializeField] private int _objectPerRow = 5;
    [SerializeField] private Vector3 _space = Vector3.one;
    [SerializeField] private List<Transform> _objects = new();

    [Button("GetObjects")]
    private void GetObjects()
    {
        _objects.Clear();
        var count = transform.childCount;
        for (var i = 0; i < count; i++)
        {
            var child = transform.GetChild(i);
            if (!child.TryGetMeshFilter(out _)) continue;
            if (!child.TryGetComponent(typeof(PivotMono), out var p))
            {
                var pivot = child.gameObject.AddComponent<PivotMono>();
                pivot.Init();
            }

            (p as PivotMono)?.Init();

            _objects.Add(child);
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
            if (i > 0 && i % _objectPerRow == 0)
            {
                previousPart = 0;
                position.x = 0;
                position.z += _space.z;
            }

            var currentPart = obj.localPosition.x - obj.GetMin().x;
            var space = i % _objectPerRow == 0 ? 0 : _space.x;
            position.x += previousPart + space + currentPart;
            obj.localPosition = position;
            previousPart = obj.GetMax().x - obj.localPosition.x;
        }
    }

    private void OnDrawGizmosSelected()
    {
        GetObjects();
    }
}