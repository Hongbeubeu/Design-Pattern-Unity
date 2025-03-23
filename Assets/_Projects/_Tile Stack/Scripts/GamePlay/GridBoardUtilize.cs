using UnityEngine;

namespace TileStack
{
    public static class GridBoardUtilize
    {
        public static Vector2Int GetDirectionVector(this Direction direction)
        {
            return direction switch
                   {
                       Direction.Forward => new Vector2Int(0, 1),
                       Direction.Backward => new Vector2Int(0, -1),
                       Direction.Left => new Vector2Int(-1, 0),
                       Direction.Right => new Vector2Int(1, 0),
                       Direction.ForwardLeft => new Vector2Int(-1, 1),
                       Direction.ForwardRight => new Vector2Int(1, 1),
                       Direction.BackwardLeft => new Vector2Int(-1, -1),
                       Direction.BackwardRight => new Vector2Int(1, -1),
                       _ => Vector2Int.zero
                   };
        }

        public static Vector3 GridToWorldPosition(this Vector2Int position, int width, int height)
        {
            var offset = new Vector3((width - 1) / 2f, 0, (height - 1) / 2f);
            return new Vector3(position.x, 0, position.y) - offset;
        }
        
        public static Vector2Int WorldToGridPosition(this Vector3 position, int width, int height)
        {
            var offset = new Vector3((width - 1) / 2f, 0, (height - 1) / 2f);
            return new Vector2Int(Mathf.RoundToInt(position.x + offset.x), Mathf.RoundToInt(position.z + offset.z));
        }
    }
}