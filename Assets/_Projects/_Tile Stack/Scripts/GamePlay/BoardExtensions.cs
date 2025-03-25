using UnityEngine;

namespace TileStack
{
    public static class BoardExtensions
    {
        public static Vector2Int GetDirectionVector(this MoveDirection moveDirection)
        {
            return moveDirection switch
                   {
                       MoveDirection.Forward => new Vector2Int(0, 1),
                       MoveDirection.Backward => new Vector2Int(0, -1),
                       MoveDirection.Left => new Vector2Int(-1, 0),
                       MoveDirection.Right => new Vector2Int(1, 0),
                       MoveDirection.ForwardLeft => new Vector2Int(-1, 1),
                       MoveDirection.ForwardRight => new Vector2Int(1, 1),
                       MoveDirection.BackwardLeft => new Vector2Int(-1, -1),
                       MoveDirection.BackwardRight => new Vector2Int(1, -1),
                       _ => Vector2Int.zero
                   };
        }
    }
}