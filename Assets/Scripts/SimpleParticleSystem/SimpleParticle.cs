using UnityEngine;

namespace SimpleParticleSystem
{
    public class SimpleParticle
    {
        private float _lifetime; // Remaining lifetime
        private float _duration; // Total lifetime
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; private set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Color Color { get; set; }
        public bool IsAlive => Lifetime > 0;


        /// <summary>
        /// Normalized lifetime value between 0 and 1
        /// </summary>
        public float LifetimeNormalized
        {
            get
            {
                var result = Lifetime / Duration;
                return result switch
                       {
                           < 0 => 0,
                           > 1 => 1,
                           _ => result
                       };
            }
        }

        public float Lifetime
        {
            get => _lifetime;
            set
            {
                if (value < 0) _lifetime = 0;
                else _lifetime = value;
            }
        }

        public float Duration
        {
            get => _duration;
            private set
            {
                if (value < 0)
                {
                    Debug.LogWarning("Duration cannot be less than 0 seconds. Set to 0.");
                    _duration = 0;
                }
                else
                    _duration = value;
            }
        }

        public SimpleParticle(Vector3 startPosition, Vector3 startVelocity, Quaternion startRotation, Vector3 startScale, float duration, Color particleColor)
        {
            Position = startPosition;
            Velocity = startVelocity;
            Rotation = startRotation;
            Scale = startScale;
            Lifetime = duration;
            Duration = duration;
            Color = particleColor;
        }
    }
}