using System;
using UnityEngine;

public class PivotMono : MonoBehaviour
{
    private Vector3 _min;
    private Vector3 _max;
    private Vector3 _center;
    private Bounds _bounds;
    
    [Button("Init")]
    private void Init()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        _bounds = mesh.bounds;
        var position = transform.position;
        _min = _bounds.min + position;
        _max = _bounds.max + position;
        _center = position;
    }

    private void Update()
    {
        _min = _bounds.min + transform.position;
        _max = _bounds.max + transform.position;
        _center = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_min, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_max, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_center, 0.1f);
    }
}
