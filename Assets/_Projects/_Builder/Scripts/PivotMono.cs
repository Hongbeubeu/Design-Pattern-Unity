using hcore.Tool;
using UnityEngine;

namespace Builder
{
    public class PivotMono : MonoBehaviour
    {
        private Bounds _totalBounds;
        private bool _isInitialized = false;

        [Button("Init")]
        public void Init()
        {
            // Lấy tất cả các MeshFilter con trong GameObject
            var meshFilters = GetComponentsInChildren<MeshFilter>();

            if (meshFilters.Length == 0)
            {
                Debug.LogWarning("Không tìm thấy MeshFilter trong GameObject hoặc các con!");
                _isInitialized = false;
                return;
            }

            // Khởi tạo bounds đầu tiên từ mesh đầu tiên
            _totalBounds = meshFilters[0].sharedMesh.bounds;
            _totalBounds = TransformBounds(meshFilters[0].transform, _totalBounds);

            // Gộp tất cả bounds của các MeshFilter con lại
            for (var i = 1; i < meshFilters.Length; i++)
            {
                var mesh = meshFilters[i].sharedMesh;
                if (mesh == null) continue;

                var meshBounds = TransformBounds(meshFilters[i].transform, mesh.bounds);
                _totalBounds.Encapsulate(meshBounds);
            }

            _isInitialized = true;
        }

        private static Bounds TransformBounds(Transform trans, Bounds localBounds)
        {
            // Chuyển đổi Bounds từ local space sang world space
            var worldCenter = trans.TransformPoint(localBounds.center);
            var worldSize = Vector3.Scale(localBounds.size, trans.lossyScale);

            return new Bounds(worldCenter, worldSize);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_isInitialized) Init();

            Gizmos.color = Color.blue;

            // Vẽ WireCube cho Bounds tổng hợp
            Gizmos.DrawWireCube(_totalBounds.center, _totalBounds.size);

            Gizmos.color = Color.red;

            // Vẽ vị trí pivot của chính object
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}