using Builder.Editor;
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
    }
}