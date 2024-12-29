using Builder.Editor;
using UnityEditor;
using UnityEngine;

namespace EzySlice
{
    public class TestDriveEzySlice : MonoBehaviour
    {
        public Plane plane = new(Vector3.zero, Vector3.up);

        [SerializeField]
        private Transform _point;

        [SerializeField]
        private GameObject _objectToSlice;

        [Button("Slice")]
        private void Slice()
        {
            _objectToSlice.SliceInstantiate(plane);
        }

        private void OnDrawGizmos()
        {
            DrawNormals();
            plane.Compute(transform);
            plane.OnDebugDraw();
            if (_point == null) return;
            var side = plane.SideOf(_point.position);

            if (side == SideOfPlane.On)
            {
                Gizmos.color = Color.yellow;
            }
            else if (side == SideOfPlane.Down)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawSphere(_point.position, 0.1f);
        }

        private Mesh _sharedMesh;

        [Button]
        private void GetSharedMesh()
        {
            if (_objectToSlice == null) return;
            var meshFilter = _objectToSlice.GetComponent<MeshFilter>();
            if (meshFilter == null) return;
            _sharedMesh = meshFilter.sharedMesh;
        }

        private void DrawNormals()
        {
            if (_objectToSlice == null) return;
            if (_sharedMesh == null) return;
            var verts = _sharedMesh.vertices;
            var uv = _sharedMesh.uv;
            var norm = _sharedMesh.normals;
            var tan = _sharedMesh.tangents;
            var tris = _sharedMesh.triangles;


            for (var i = 0; i < tris.Length; i++)
            {
                var index = tris[i];
                var point = _objectToSlice.transform.TransformPoint(verts[index]);
                var normal = _objectToSlice.transform.TransformDirection(norm[index]);
                var tangent = _objectToSlice.transform.TransformDirection(tan[index]);
                var p = point + normal * 0.04f;
                var q = point + tangent * 0.04f;
                var r = point + Vector3.Cross(normal, tangent) * 0.01f;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(point, p);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(point, q);
                Gizmos.color = Color.blue;
                Handles.color = Color.black;
                Gizmos.DrawLine(point, r);
            }
        }
    }
}