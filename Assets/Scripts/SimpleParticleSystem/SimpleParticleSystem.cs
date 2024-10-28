using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SimpleParticleSystem
{
    public partial class SimpleParticleSystem : MonoBehaviour
    {
        public float duration = 3f;
        public bool looping;

        public RandomNumberFloat startDelay = new();
        public RandomNumberFloat startLifetime = new();
        public RandomNumberFloat startSpeed = new();

        [Header("Emission")] public bool playOnAwake;
        public int maxParticles = 100;
        public float emissionRate = 5f; // Particles per second
        public Color particleColor = Color.white;
        public bool isPaused;

        private readonly List<SimpleParticle> _particles = new();
        private MaterialPropertyBlock _propertyBlock;
        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;

        private float _timeSinceLastEmission;
        private float _lifetime;
        private bool _isInitialized;
        private float _delayCounter;
        private float EmissionInterval => 1 / emissionRate;

        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Preload();
            if (playOnAwake)
            {
                Init();
            }
        }

        private void Update()
        {
            if (!_isInitialized) return;

            if (_delayCounter > 0)
            {
                _delayCounter -= Time.deltaTime;
                return;
            }


            EmitParticles();
            UpdateParticles();
            RenderParticles();
        }

        private void Preload()
        {
            if (particleMesh == null)
                particleMesh = CreateQuadMesh();
            _propertyBlock = new MaterialPropertyBlock();
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
            _matrices.Clear();
            _colors.Clear();
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
            if (_particles.Count > maxParticles)
            {
                RemoveParticles(0, _particles.Count - maxParticles);
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

            //TODO: Implement shape emission
            var position = transform.position;
            var velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            var particle = SimpleParticlePooler.GetParticle();
            particle.Initialize(position, velocity * startSpeed.GetValue(), Quaternion.identity, Vector3.one, startLifetime.GetValue(), particleColor);
            _particles.Add(particle);
            _timeSinceLastEmission = 0;
        }

        private void UpdateParticles()
        {
            if (isPaused) return;

            for (var i = _particles.Count - 1; i >= 0; i--)
            {
                var particle = _particles[i];
                particle.Lifetime -= Time.deltaTime;
                if (particle.IsAlive)
                {
                    particle.Position += particle.Velocity * Time.deltaTime;
                    var currentRotation = particle.Rotation.eulerAngles;
                    currentRotation.z += 360 * Time.deltaTime;
                    particle.Rotation = Quaternion.Euler(currentRotation);
                    particle.Scale = Vector3.one * (1 - particle.LifetimeNormalized);
                    var color = particle.Color;
                    color.a = particle.LifetimeNormalized;
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
                SimpleParticlePooler.ReturnParticle(_particles[i]);
            }

            _particles.RemoveRange(fromIndex, toIndex - fromIndex);
        }

        private void RemoveParticle(int index)
        {
            SimpleParticlePooler.ReturnParticle(_particles[index]);
            _particles.RemoveAt(index);
        }

        private void RemoveParticle(SimpleParticle particle)
        {
            SimpleParticlePooler.ReturnParticle(particle);
            _particles.Remove(particle);
        }

        private void RemoveAllParticles()
        {
            RemoveParticles(0, _particles.Count);
        }

        private readonly List<Matrix4x4> _matrices = new();

        private readonly List<Vector4> _colors = new();

        private void RenderParticles()
        {
            const int batchSize = 511;
            _matrices.Clear();
            _colors.Clear();
            _propertyBlock.Clear();

            for (var i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                // Tạo matrix cho từng particle
                var matrix = Matrix4x4.TRS(particle.Position, particle.Rotation, particle.Scale);
                _matrices.Add(matrix);

                // Thêm màu của particle vào danh sách
                var currentColor = (Vector4)particle.Color;
                _colors.Add(currentColor);

                // Vẽ particles theo batch size
                if (_matrices.Count == batchSize || i == _particles.Count - 1)
                {
                    // Gán danh sách màu vào MaterialPropertyBlock
                    _propertyBlock.SetVectorArray(ColorId, _colors);

                    // Render batch hiện tại
                    Graphics.RenderMeshInstanced(
                        new RenderParams(particleMaterial) { matProps = _propertyBlock },
                        particleMesh,
                        0,
                        _matrices
                    );

                    // Xóa matrices và colors sau mỗi batch
                    _matrices.Clear();
                    _colors.Clear();
                }
            }
        }

        private static Mesh CreateQuadMesh()
        {
            var mesh = new Mesh
                       {
                           vertices = new[]
                                      {
                                          new Vector3(-0.5f, -0.5f, 0),
                                          new Vector3(0.5f, -0.5f, 0),
                                          new Vector3(-0.5f, 0.5f, 0),
                                          new Vector3(0.5f, 0.5f, 0)
                                      },
                           uv = new[]
                                {
                                    new Vector2(0, 0),
                                    new Vector2(1, 0),
                                    new Vector2(0, 1),
                                    new Vector2(1, 1)
                                },
                           triangles = new[] { 0, 2, 1, 2, 3, 1 }
                       };
            return mesh;
        }
    }
}