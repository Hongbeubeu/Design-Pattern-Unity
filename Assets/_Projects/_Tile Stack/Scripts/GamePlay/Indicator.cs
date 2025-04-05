using UnityEngine;

namespace TileStack
{
    public class Indicator : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;

        public void UpdateIndicator(MoveDirection moveDirection)
        {
            if (this == null) return;
            if (_indicator == null) return;

            if (moveDirection == MoveDirection.None)
            {
                _indicator.SetActive(false);

                return;
            }

            _indicator.SetActive(true);
            var directionVector = moveDirection.GetDirectionVector();
            _indicator.transform.forward = new Vector3(directionVector.x, 0, directionVector.y);
        }
    }
}