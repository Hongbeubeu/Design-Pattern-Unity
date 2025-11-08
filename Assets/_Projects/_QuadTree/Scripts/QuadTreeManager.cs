using System.Collections.Generic;
using UnityEngine;

public class QuadTreeManager : MonoBehaviour
{
    public Rect worldBounds = new Rect(-50, -50, 100, 100);
    public int maxObjectsPerNode = 5;
    public int maxDepth = 8;

    public bool drawGizmos = true;

    private QuadTree quadTree;
    private List<MovingObject> allObjects;

    void Awake()
    {
        quadTree = new QuadTree(worldBounds, maxObjectsPerNode, maxDepth);
        allObjects = new List<MovingObject>();
    }

    // Các đối tượng tự đăng ký
    public void RegisterObject(MovingObject obj)
    {
        if (!allObjects.Contains(obj))
        {
            allObjects.Add(obj);
        }
    }

    // Các đối tượng tự hủy đăng ký
    public void UnregisterObject(MovingObject obj)
    {
        if (allObjects.Contains(obj))
        {
            allObjects.Remove(obj);
        }
    }

    // Sử dụng LateUpdate để đảm bảo tất cả các đối tượng đã di chuyển xong trong Update()
    void LateUpdate()
    {
        // 1. Xóa cây cũ
        quadTree.Clear();

        // 2. Chèn lại tất cả các đối tượng vào vị trí MỚI của chúng
        foreach (MovingObject obj in allObjects)
        {
            if(obj.isActiveAndEnabled)
            {
                quadTree.Insert(obj);
            }
        }
    }

    /// <summary>
    /// Hàm này có thể được gọi từ bất kỳ script nào khác để tìm các đối tượng
    /// </summary>
    public List<IQuadTreeObject> FindObjectsInArea(Rect queryArea)
    {
        return quadTree.Query(queryArea);
    }
    
    /// <summary>
    /// Trả về (Rect, Depth) của nút Quadtree mà một đối tượng thuộc về.
    /// </summary>
    public (Rect bounds, int depth) GetNodeInfoForObject(IQuadTreeObject obj)
    {
        if (quadTree != null)
        {
            return quadTree.GetNodeInfoForObject(obj);
        }
        return (Rect.zero, -1);
    }

    /// <summary>
    /// Trả về maxDepth để các script khác có thể dùng để tô màu Gizmo.
    /// </summary>
    public float GetMaxDepth()
    {
        return this.maxDepth;
    }

    // Vẽ Quadtree để gỡ lỗi
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            // Vẽ biên giới thế giới
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(worldBounds.center.x, 0, worldBounds.center.y);
            Vector3 size = new Vector3(worldBounds.width, 0.1f, worldBounds.height);
            Gizmos.DrawWireCube(center, size);

            // Vẽ các nút của cây
            if (quadTree != null)
            {
                quadTree.DrawGizmos();
            }
        }
    }
}