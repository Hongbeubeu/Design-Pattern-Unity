using System;
using UnityEngine;

namespace TileStack
{
    [RequireComponent(typeof(StackTile))]
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private StackTile _tile;

        private void OnMouseDown()
        {
            if (_tile == null)
            {
                throw new NullReferenceException($"No tile assigned to {gameObject.name}");
            }

            if (_tile.Movement.IsMoving) return;
            if (GameController.Instance.IsAnyTileMoving()) return;
            if (_tile.MoveDirection == MoveDirection.None) return;
            _tile.Move();
        }
    }
}