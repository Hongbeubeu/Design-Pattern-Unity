using System;
using UnityEngine;

public static class CommonExtensions
{
    public static Vector3 GetMin(this Transform transform)
    {
        var mesh = transform.GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            throw new ArgumentException($"Transform {transform.name} does not have a MeshFilter component.");
        }

        var bounds = mesh.bounds;
        var position = transform.position;
        return bounds.min + position;
    }
    
    public static bool TryGetMin(this Transform transform, out Vector3 min)
    {
        min = Vector3.zero;
        var mesh = transform.GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            return false;
        }

        var bounds = mesh.bounds;
        var position = transform.position;
        min = bounds.min + position;
        return true;
    }
    
    public static Vector3 GetMax(this Transform transform)
    {
        var mesh = transform.GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            throw new ArgumentException($"Transform {transform.name} does not have a MeshFilter component.");
        }

        var bounds = mesh.bounds;
        var position = transform.position;
        return bounds.max + position;
    }
    
    public static bool TryGetMax(this Transform transform, out Vector3 max)
    {
        max = Vector3.zero;
        var mesh = transform.GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null)
        {
            return false;
        }

        var bounds = mesh.bounds;
        var position = transform.position;
        max = bounds.max + position;
        return true;
    }
    
    public static bool TryGetMeshFilter(this Transform transform, out MeshFilter meshFilter)
    {
        meshFilter = transform.GetComponent<MeshFilter>();
        return meshFilter != null;
    }
}