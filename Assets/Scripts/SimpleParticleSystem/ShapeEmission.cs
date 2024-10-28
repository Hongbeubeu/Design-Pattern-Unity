using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SimpleParticleSystem
{
    [Serializable]
    public class ShapeEmission
    {
        public ShapeEmissionType shapeEmissionType;
        public Vector3 position;
        public Vector3 size;
        public float radius;

        public Vector3 GetRandomPosition()
        {
            return shapeEmissionType switch
                   {
                       ShapeEmissionType.Point => position,
                       ShapeEmissionType.Circle => GetRandomPointInCircle(),
                       ShapeEmissionType.Sphere => GetRandomPointInSphere(),
                       ShapeEmissionType.Box => GetRandomPointInBox(),
                       _ => position
                   };
        }

        public Vector3 GetRandomDirection()
        {
            return shapeEmissionType switch
                   {
                       ShapeEmissionType.Point => Vector3.zero,
                       ShapeEmissionType.Circle => GetRandomDirectionInCircle(),
                       ShapeEmissionType.Sphere => GetRandomDirectionInSphere(),
                       ShapeEmissionType.Box => GetRandomDirectionInBox(),
                       _ => Vector3.zero
                   };
        }

        private Vector3 GetRandomDirectionInBox()
        {
            // Random direction in box
            var x = Random.Range(-1, 1);
            var y = Random.Range(-1, 1);
            var z = Random.Range(-1, 1);
            return new Vector3(x, y, z).normalized;
        }

        private Vector3 GetRandomDirectionInSphere()
        {
            // Random direction in sphere
            var randomDirection = Random.insideUnitSphere;
            return randomDirection.normalized;
        }

        private Vector3 GetRandomDirectionInCircle()
        {
            // Random direction in circle
            var randomDirection = Random.insideUnitCircle;
            return new Vector3(randomDirection.x, randomDirection.y, 0).normalized;
        }

        private Vector3 GetRandomPointInBox()
        {
            // Random point in box
            var x = Random.Range(-size.x / 2, size.x / 2);
            var y = Random.Range(-size.y / 2, size.y / 2);
            var z = Random.Range(-size.z / 2, size.z / 2);
            return position + new Vector3(x, y, z);
        }

        private Vector3 GetRandomPointInSphere()
        {
            // Random point in sphere
            var randomDirection = Random.insideUnitSphere;
            return position + randomDirection * radius;
        }

        private Vector3 GetRandomPointInCircle()
        {
            // Random point in circle
            var randomDirection = Random.insideUnitCircle;
            return position + new Vector3(randomDirection.x, randomDirection.y, 0) * radius;
        }
    }
}