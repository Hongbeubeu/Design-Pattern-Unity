using System.Collections.Generic;
using UnityEngine;

namespace hcore.SimpleParticleSystem
{
    public partial class SimpleParticleSystem : MonoBehaviour
    {
        public float duration = 3f;
        public bool looping;

        public RandomNumberFloat startDelay = new();
        public RandomNumberFloat startLifetime = new();
        public RandomNumberFloat startSpeed = new();
        public RandomNumberFloat startSize = new();
        public RandomNumberFloat startRotation = new();
        public bool overrideDirection = true;
        public RandomVector3 startDirection = new();
        public Gradient colorOverLifetime;
        public AnimationCurve sizeOverLifetime;
        public ShapeEmission shapeEmission;
        public float rotateSpeed;

        [Header("Emission")] public bool playOnAwake;
        public int maxParticles = 100;
        public float emissionRate = 5f; // Particles per second
        public bool isPaused;

        [SerializeField] public Mesh particleMesh;
        [SerializeField] public Material particleMaterial;

        private float _timeSinceLastEmission;
        private float _lifetime;
        private bool _isInitialized;
        private float _delayCounter;
        private float EmissionInterval => 1 / emissionRate;

        public List<SimpleParticle> Particles { get; } = new();

        private void Start()
        {
            if (playOnAwake)
            {
                Init();
            }

            SimpleParticleSystemRenderer.AddParticleSystem(this);
        }

        public void DoUpdate()
        {
            if (!_isInitialized) return;

            if (_delayCounter > 0)
            {
                _delayCounter -= Time.deltaTime;
                return;
            }


            EmitParticles();
            UpdateParticles();
        }

        private void OnDestroy()
        {
            RemoveAllParticles();
            SimpleParticleSystemRenderer.RemoveParticleSystem(this);
        }

        public void Play()
        {
            if (_isInitialized)
            {
                Stop(true);
            }

            Init();
        }

        public void Stop(bool clearParticles = false)
        {
            if (clearParticles)
            {
                RemoveAllParticles();
                _isInitialized = false;
            }

            _lifetime = 0;
        }

        private void Init()
        {
            if (particleMaterial == null)
            {
                Debug.LogError("Particle material is not assigned. Disabling the script.");
                enabled = false;
                return;
            }

            _delayCounter = startDelay.GetValue();
            _lifetime = duration;
            _isInitialized = true;
        }

        private void EmitParticles()
        {
            if (isPaused) return;

            // Increase lifetime if not looping
            if (!looping)
            {
                _lifetime -= Time.deltaTime;
            }

            // Limit the number of particles
            if (Particles.Count > maxParticles)
            {
                RemoveParticles(0, Particles.Count - maxParticles);
                return;
            }

            // Check if enough time has passed to emit a new particle
            if (_timeSinceLastEmission < EmissionInterval)
            {
                _timeSinceLastEmission += Time.deltaTime;
                return;
            }

            // Stop emitting particles if the lifetime is greater than the duration
            if (_lifetime <= 0)
            {
                return;
            }

            var position = transform.position + shapeEmission.GetRandomPosition();
            var velocity = overrideDirection ? startDirection.GetValue() : shapeEmission.GetRandomDirection();
            var particle = SimpleParticlePooler.GetParticle();
            particle.Initialize(position, velocity * startSpeed.GetValue(), Quaternion.Euler(0, 0, startRotation.GetValue()), Vector3.one * sizeOverLifetime.Evaluate(0), startLifetime.GetValue(), colorOverLifetime.Evaluate(0));
            Particles.Add(particle);
            _timeSinceLastEmission = 0;
        }

        private void UpdateParticles()
        {
            if (isPaused) return;

            for (var i = Particles.Count - 1; i >= 0; i--)
            {
                var particle = Particles[i];
                particle.Lifetime -= Time.deltaTime;
                if (particle.IsAlive)
                {
                    particle.Position += particle.Velocity * Time.deltaTime;
                    var currentRotation = particle.Rotation.eulerAngles;
                    currentRotation.z += rotateSpeed * Time.deltaTime;
                    particle.Rotation = Quaternion.Euler(currentRotation);
                    particle.Scale = Vector3.one * (sizeOverLifetime.Evaluate(1 - particle.LifetimeNormalized) * startSize.GetValue());
                    var color = colorOverLifetime.Evaluate(1 - particle.LifetimeNormalized);
                    particle.Color = color;
                }
                else
                {
                    RemoveParticle(particle);
                }
            }
        }

        private void RemoveParticles(int fromIndex, int toIndex)
        {
            for (var i = fromIndex; i < toIndex; i++)
            {
                SimpleParticlePooler.ReturnParticle(Particles[i]);
            }

            Particles.RemoveRange(fromIndex, toIndex - fromIndex);
        }

        private void RemoveParticle(int index)
        {
            SimpleParticlePooler.ReturnParticle(Particles[index]);
            Particles.RemoveAt(index);
        }

        private void RemoveParticle(SimpleParticle particle)
        {
            SimpleParticlePooler.ReturnParticle(particle);
            Particles.Remove(particle);
        }

        private void RemoveAllParticles()
        {
            RemoveParticles(0, Particles.Count);
        }

        
    }
}