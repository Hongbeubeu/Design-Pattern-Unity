using UnityEngine;

namespace SimpleParticleSystem
{
    public class SimpleParticle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float lifeTime;
        public float maxLifeTime;
        public Color color;

        public bool IsAlive => lifeTime > 0;

        public SimpleParticle(Vector3 startPosition, Vector3 startVelocity, float life, Color particleColor)
        {
            position = startPosition;
            velocity = startVelocity;
            lifeTime = life;
            maxLifeTime = life;
            color = particleColor;
        }
    }
}