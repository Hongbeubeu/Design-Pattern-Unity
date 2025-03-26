using UnityEngine;

namespace hcore.Extensions
{
    public static class TransformExtension
    {
        public static void Reset(this Transform transform)
        {
            transform.ResetPosition();
            transform.ResetRotation();
            transform.ResetScale();
        }

        public static void ResetPosition(this Transform transform)
        {
            transform.position = Vector3.zero;
        }

        public static void ResetRotation(this Transform transform)
        {
            transform.rotation = Quaternion.identity;
        }

        public static void ResetScale(this Transform transform)
        {
            transform.localScale = Vector3.one;
        }

        public static Transform FindChildByName(this Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }

                var result = child.FindChildByName(name);

                if (result != null) return result;
            }

            return null;
        }

        public static float DistanceTo(this Transform transform, Transform target)
        {
            return Vector3.Distance(transform.position, target.position);
        }
    }
}