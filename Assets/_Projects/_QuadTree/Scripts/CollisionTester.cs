using System.Collections.Generic;
using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    [SerializeField] private Vector2 _size = new(1f, 1f);
    private QuadTreeManager manager;

    void Start()
    {
        manager = FindObjectOfType<QuadTreeManager>();
    }

    void Update()
    {
        // Tạo một khu vực truy vấn (ví dụ: xung quanh chính đối tượng này)
        Vector3 pos = transform.position;
        Rect queryRect = new Rect(pos.x - _size.x/2f, pos.z - _size.y/2f, _size.x, _size.y);
        
        // Vẽ gizmo cho khu vực truy vấn
        DebugDrawRect(queryRect, Color.yellow);

        // Thực hiện truy vấn
        List<IQuadTreeObject> nearbyObjects = manager.FindObjectsInArea(queryRect);

        // (nearbyObjects) bây giờ chứa MỌI đối tượng (bao gồm cả chính nó)
        // trong bán kính đó, thay vì phải kiểm tra với TẤT CẢ đối tượng trong game.
        
        if (nearbyObjects.Count > 0)
            Debug.Log($"Tìm thấy {nearbyObjects.Count} đối tượng ở gần.");

        // Bạn có thể thực hiện kiểm tra va chạm chi tiết (ví dụ: hình tròn)
        // chỉ với các đối tượng trong danh sách này.
        foreach (IQuadTreeObject obj in nearbyObjects)
        {
            if (obj == this.GetComponent<IQuadTreeObject>()) continue; // Bỏ qua chính mình
            
            // (Làm gì đó với đối tượng, ví dụ: (MovingObject)obj).transform.position)
        }
    }
    
    void DebugDrawRect(Rect rect, Color color)
    {
        Vector3 p1 = new Vector3(rect.xMin, 0, rect.yMin);
        Vector3 p2 = new Vector3(rect.xMax, 0, rect.yMin);
        Vector3 p3 = new Vector3(rect.xMax, 0, rect.yMax);
        Vector3 p4 = new Vector3(rect.xMin, 0, rect.yMax);
        Debug.DrawLine(p1, p2, color);
        Debug.DrawLine(p2, p3, color);
        Debug.DrawLine(p3, p4, color);
        Debug.DrawLine(p4, p1, color);
    }
}