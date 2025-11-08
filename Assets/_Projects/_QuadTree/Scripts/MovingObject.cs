using System;
using UnityEngine;

public class MovingObject : MonoBehaviour, IQuadTreeObject
{
    // Kích thước 2D của đối tượng trên mặt phẳng Oxz
    public float sizeX = 1f;
    public float sizeZ = 1f;

    // (Tùy chọn) Để dễ dàng gỡ lỗi, hãy lưu trữ tham chiếu đến trình quản lý
    [HideInInspector] public QuadTreeManager manager;
    private Rect _bounds;

    void Start()
    {
        // Tự động tìm và đăng ký với trình quản lý
        manager = FindObjectOfType<QuadTreeManager>();

        if (manager != null)
        {
            manager.RegisterObject(this);
        }
    }

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.UnregisterObject(this);
        }
    }

    /// <summary>
    /// Triển khai giao diện IQuadTreeObject
    /// </summary>
    public Rect GetBounds()
    {
        Vector3 pos = transform.position;
        // Tính toán Rect (x, y, width, height)
        // x = pos.x - (nửa chiều rộng)
        // y = pos.z - (nửa chiều cao)
        _bounds.x = pos.x - sizeX * 0.5f;
        _bounds.y = pos.z - sizeZ * 0.5f; // y của Rect là z của thế giới
        _bounds.width = sizeX;
        _bounds.height = sizeZ;

        return _bounds;
    }

    // (Tùy chọn) Vẽ Gizmos cho biên của đối tượng
    private void OnDrawGizmos()
    {
        Rect bounds = GetBounds();
        Vector3 center = new Vector3(bounds.center.x, transform.position.y, bounds.center.y);
        Vector3 size = new Vector3(bounds.width, 1f, bounds.height);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }

    private void OnDrawGizmosSelected()
    {
        // Đảm bảo chúng ta có tham chiếu đến manager
        // (Nó có thể bị mất khi vào/thoát Play Mode)
        if (manager == null)
        {
            manager = FindObjectOfType<QuadTreeManager>();
            if (manager == null) return; // Không tìm thấy manager, không thể vẽ
        }

        // 1. Yêu cầu manager tìm thông tin nút
        var nodeInfo = manager.GetNodeInfoForObject(this);

        // 2. Kiểm tra xem có tìm thấy không (depth != -1)
        if (nodeInfo.depth != -1)
        {
            // 3. Lấy maxDepth để tính toán màu sắc
            float maxDepth = manager.GetMaxDepth();
            if (maxDepth == 0) maxDepth = 1; // Tránh chia cho 0

            // 4. Đặt màu Gizmo giống hệt như trong QuadTreeNode.DrawGizmos()
            // Chúng ta dùng một màu sáng hơn một chút (ví dụ: Color.cyan) để làm nổi bật
            Color baseColor = Color.Lerp(Color.white, Color.blue, (float)nodeInfo.depth / maxDepth);
            Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f); // Thêm chút trong suốt

            // 5. Vẽ biên của nút đó
            Rect nodeBounds = nodeInfo.bounds;
            Vector3 center = new Vector3(nodeBounds.center.x, transform.position.y, nodeBounds.center.y);
            
            // Vẽ một khối đặc (Solid Cube) để dễ thấy hơn
            Vector3 size = new Vector3(nodeBounds.width, 0.05f, nodeBounds.height);
            Gizmos.DrawCube(center, size);
            
            // Và vẽ lại đường viền (Wire Cube) với màu đậm
            Gizmos.color = baseColor;
            Gizmos.DrawWireCube(center, size);
        }
    }
}