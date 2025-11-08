using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private QuadTreeNode root;
    private int maxObjectsPerNode;
    private int maxDepth;

    /// <summary>
    /// Khởi tạo Quadtree.
    /// </summary>
    /// <param name="bounds">Biên giới tổng thể của thế giới (trên mặt phẳng Oxz).</param>
    /// <param name="maxObjectsPerNode">Số lượng đối tượng tối đa mỗi nút trước khi chia nhỏ.</param>
    /// <param name="maxDepth">Độ sâu tối đa của cây.</param>
    public QuadTree(Rect bounds, int maxObjectsPerNode, int maxDepth)
    {
        this.root = new QuadTreeNode(bounds, 0, maxObjectsPerNode, maxDepth);
        this.maxObjectsPerNode = maxObjectsPerNode;
        this.maxDepth = maxDepth;
    }

    /// <summary>
    /// Xóa tất cả các đối tượng và nút con khỏi cây.
    /// </summary>
    public void Clear()
    {
        root.Clear();
    }

    /// <summary>
    /// Chèn một đối tượng vào cây.
    /// </summary>
    public void Insert(IQuadTreeObject obj)
    {
        root.Insert(obj);
    }

    /// <summary>
    /// Lấy tất cả các đối tượng có thể va chạm với một khu vực nhất định.
    /// </summary>
    public List<IQuadTreeObject> Query(Rect area)
    {
        return root.Query(area);
    }
    
    /// <summary>
    /// (Tùy chọn) Vẽ Gizmos để gỡ lỗi trong Scene View.
    /// </summary>
    public void DrawGizmos()
    {
        root.DrawGizmos();
    }
    
    /// <summary>
    /// Lấy thông tin (Bounds, Depth) của nút chứa một đối tượng cụ thể.
    /// </summary>
    public (Rect bounds, int depth) GetNodeInfoForObject(IQuadTreeObject obj)
    {
        return root.FindNodeInfo(obj);
    }

    // Lớp Node (Nút) của Quadtree
    private class QuadTreeNode
    {
        private Rect bounds;
        private int depth;
        private int maxObjectsPerNode;
        private int maxDepth;

        private List<IQuadTreeObject> objects;
        private QuadTreeNode[] children;
        private bool isLeaf => children == null;

        public QuadTreeNode(Rect bounds, int depth, int maxObjects, int maxDepth)
        {
            this.bounds = bounds;
            this.depth = depth;
            this.maxObjectsPerNode = maxObjects;
            this.maxDepth = maxDepth;
            this.objects = new List<IQuadTreeObject>();
            this.children = null; // Bắt đầu là một lá (leaf)
        }

        public void Clear()
        {
            objects.Clear();
            if (!isLeaf)
            {
                foreach (var child in children)
                {
                    child.Clear();
                }
            }
            children = null; // Trở thành lá
        }

        public void Insert(IQuadTreeObject obj)
        {
            // Nếu chúng ta không phải là lá, hãy thử chèn vào các con
            if (!isLeaf)
            {
                int index = GetChildIndex(obj.GetBounds());

                if (index != -1)
                {
                    // Đối tượng nằm hoàn toàn trong một nút con
                    children[index].Insert(obj);
                    return;
                }
            }

            // Nếu chúng ta là lá, hoặc đối tượng nằm chồng lên ranh giới con
            // --> Thêm đối tượng vào nút này
            objects.Add(obj);

            // Nếu chúng ta là lá và bây giờ đã đầy, hãy chia nhỏ
            if (isLeaf && objects.Count > maxObjectsPerNode && depth < maxDepth)
            {
                Subdivide();

                // Chuyển các đối tượng từ nút này xuống các con
                List<IQuadTreeObject> objectsToMove = new List<IQuadTreeObject>(objects);
                objects.Clear();

                foreach (var item in objectsToMove)
                {
                    Insert(item); // Chèn lại (giờ sẽ đi vào các con)
                }
            }
        }

        public List<IQuadTreeObject> Query(Rect queryArea)
        {
            List<IQuadTreeObject> results = new List<IQuadTreeObject>();

            // 1. Nếu khu vực truy vấn không giao với nút này, hãy dừng lại
            if (!bounds.Overlaps(queryArea))
            {
                return results; // Trả về danh sách rỗng
            }

            // 2. Lấy tất cả các đối tượng trong nút này
            foreach (var obj in objects)
            {
                if (queryArea.Overlaps(obj.GetBounds()))
                {
                    results.Add(obj);
                }
            }

            // 3. Nếu chúng ta không phải là lá, đệ quy xuống các con
            if (!isLeaf)
            {
                foreach (var child in children)
                {
                    results.AddRange(child.Query(queryArea));
                }
            }

            return results;
        }
        
        /// <summary>
        /// Tìm (Rect, depth) của nút chứa đối tượng này.
        /// Trả về (Rect.zero, -1) nếu không tìm thấy.
        /// </summary>
        public (Rect bounds, int depth) FindNodeInfo(IQuadTreeObject target)
        {
            // 1. Tối ưu hóa: Nếu đối tượng không thể ở trong nút này, hãy dừng lại.
            // (Chúng ta dùng GetBounds() thay vì queryArea)
            if (!bounds.Overlaps(target.GetBounds()))
            {
                return (Rect.zero, -1); // (-1) = không tìm thấy
            }

            // 2. Kiểm tra xem đối tượng có được lưu trữ TRỰC TIẾP trong nút này không
            // (Đây là điều kiện quan trọng: nó nằm trong danh sách 'objects')
            if (objects.Contains(target))
            {
                return (this.bounds, this.depth); // Tìm thấy!
            }

            // 3. Nếu không có ở đây, và chúng ta có các con, hãy hỏi chúng
            if (!isLeaf)
            {
                foreach (var child in children)
                {
                    var result = child.FindNodeInfo(target);
                    if (result.depth != -1) // Nếu một đứa con tìm thấy nó
                    {
                        return result; // Trả về kết quả đó
                    }
                }
            }

            // 4. Không có trong nút này, và cũng không có trong các con.
            return (Rect.zero, -1);
        }

        private void Subdivide()
        {
            float halfWidth = bounds.width / 2;
            float halfHeight = bounds.height / 2;
            float x = bounds.x;
            float z = bounds.y; // Nhớ rằng y của Rect là z của thế giới

            children = new QuadTreeNode[4];
            
            // Tọa độ Rect (x, y) là góc trên bên trái (Top-Left)
            // Trong hệ tọa độ của chúng ta, "y" (tức là z) tăng dần xuống dưới (hoặc lên trên, tùy quy ước)
            // Hãy thống nhất: 
            // 0: Tây Bắc (Top-Left)
            // 1: Đông Bắc (Top-Right)
            // 2: Tây Nam (Bottom-Left)
            // 3: Đông Nam (Bottom-Right)

            var nwBounds = new Rect(x, z, halfWidth, halfHeight);
            var neBounds = new Rect(x + halfWidth, z, halfWidth, halfHeight);
            var swBounds = new Rect(x, z + halfHeight, halfWidth, halfHeight);
            var seBounds = new Rect(x + halfWidth, z + halfHeight, halfWidth, halfHeight);

            children[0] = new QuadTreeNode(nwBounds, depth + 1, maxObjectsPerNode, maxDepth);
            children[1] = new QuadTreeNode(neBounds, depth + 1, maxObjectsPerNode, maxDepth);
            children[2] = new QuadTreeNode(swBounds, depth + 1, maxObjectsPerNode, maxDepth);
            children[3] = new QuadTreeNode(seBounds, depth + 1, maxObjectsPerNode, maxDepth);
        }

        /// <summary>
        /// Trả về chỉ số của nút con mà đối tượng nằm HOÀN TOÀN bên trong.
        /// Trả về -1 nếu nó nằm chồng lên ranh giới.
        /// </summary>
        private int GetChildIndex(Rect objBounds)
        {
            // Nếu không phải là lá, không thể chứa
            if(isLeaf) return -1;

            float midX = bounds.x + bounds.width / 2;
            float midZ = bounds.y + bounds.height / 2; // y của Rect là z

            bool inNorth = objBounds.yMax < midZ; // Nằm hoàn toàn ở phía Bắc (trên)
            bool inSouth = objBounds.yMin > midZ; // Nằm hoàn toàn ở phía Nam (dưới)
            bool inWest = objBounds.xMax < midX;  // Nằm hoàn toàn ở phía Tây (trái)
            bool inEast = objBounds.xMin > midX;  // Nằm hoàn toàn ở phía Đông (phải)

            if (inNorth && inWest) return 0;
            if (inNorth && inEast) return 1;
            if (inSouth && inWest) return 2;
            if (inSouth && inEast) return 3;

            return -1; // Nằm chồng
        }

        public void DrawGizmos()
        {
            // Vẽ biên của nút này
            Gizmos.color = Color.Lerp(Color.white, Color.blue, (float)depth / maxDepth);
            Vector3 center = new Vector3(bounds.center.x, 0, bounds.center.y);
            Vector3 size = new Vector3(bounds.width, 0.1f, bounds.height);
            Gizmos.DrawWireCube(center, size);

            // Vẽ các con
            if (!isLeaf)
            {
                foreach (var child in children)
                {
                    child.DrawGizmos();
                }
            }
        }
    }
}