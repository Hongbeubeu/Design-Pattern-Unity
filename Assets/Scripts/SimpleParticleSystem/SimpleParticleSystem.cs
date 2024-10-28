using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SimpleParticleSystem
{
    public partial class SimpleParticleSystem : MonoBehaviour
    {
        public bool playOnAwake;
        public bool loop;
        public int maxParticles = 100;
        public float emissionRate = 5f; // Particles per second
        public float duration = 3f;
        public Color particleColor = Color.white;
        public bool isPaused;

        private readonly List<SimpleParticle> _particles = new();
        private MaterialPropertyBlock _propertyBlock;
        [SerializeField] private Mesh particleMesh;
        [SerializeField] private Material particleMaterial;

        private float _timeSinceLastEmission;
        private float _lifetime;
        private bool _isInitialized;
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
                _particles.Clear();
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

            _lifetime = duration;
            _matrices.Clear();
            _colors.Clear();
            _isInitialized = true;
        }

        private void EmitParticles()
        {
            if (isPaused) return;

            // Increase lifetime if not looping
            if (!loop)
            {
                _lifetime -= Time.deltaTime;
            }

            // Limit the number of particles
            if (_particles.Count > maxParticles)
            {
                _particles.RemoveRange(0, _particles.Count - maxParticles);
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
            particle.Initialize(position, velocity, Quaternion.identity, Vector3.one, duration, particleColor);
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
                    SimpleParticlePooler.ReturnParticle(particle);
                    _particles.RemoveAt(i);
                }
            }
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