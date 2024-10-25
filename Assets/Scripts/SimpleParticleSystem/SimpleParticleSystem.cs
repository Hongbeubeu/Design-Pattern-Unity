using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace SimpleParticleSystem
{
    public class SimpleParticleSystem : MonoBehaviour
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly int Scale = Shader.PropertyToID("_Scale");

        public int particleCount = 100;
        public float emissionRate = 5f;
        public float particleLifeTime = 3f;
        public Color particleColor = Color.white;
        public bool pause;

        private readonly List<SimpleParticle> _particles = new();
        private Mesh _particleMesh;
        [SerializeField] private Material _particleMaterial;
        private MaterialPropertyBlock _propertyBlock;


        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (_particleMesh == null || _propertyBlock == null)
            {
                Init();
            }

            EmitParticles();
            UpdateParticles();
            RenderParticles();
        }

        private void Init()
        {
            Application.targetFrameRate = 60;
            _particleMesh = CreateQuadMesh();
            _propertyBlock = new MaterialPropertyBlock();
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

        private void EmitParticles()
        {
            if (_particles.Count > 0) return;
            for (var i = 0; i < emissionRate; i++)
            {
                var position = transform.position;
                var velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                _particles.Add(new SimpleParticle(position, velocity, Quaternion.identity, Vector3.zero, particleLifeTime, particleColor));
            }
        }

        private void UpdateParticles()
        {
            if (pause) return;

            for (var i = _particles.Count - 1; i >= 0; i--)
            {
                var particle = _particles[i];
                particle.lifeTime -= Time.deltaTime;
                if (particle.IsAlive)
                {
                    particle.position += particle.velocity * Time.deltaTime;
                    var currentRotation = particle.rotation.eulerAngles;
                    currentRotation.z += 360 * Time.deltaTime;
                    particle.rotation = Quaternion.Euler(currentRotation);
                    particle.scale = Vector3.one * (1 - particle.lifeTime / particle.maxLifeTime);
                }
                else
                {
                    _particles.RemoveAt(i);
                }
            }
        }

        private void RenderParticles()
        {
            _particleMesh = CreateQuadMesh();
            _propertyBlock.Clear();
            const int batchSize = 1023;
            var matrices = new List<Matrix4x4>();
            var colors = new List<Vector4>();
            var scales = new List<float>();

            for (var i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                // Tạo matrix cho từng particle
                var matrix = Matrix4x4.TRS(particle.position, particle.rotation, particle.scale);
                matrices.Add(matrix);

                // Tạo màu sắc dựa trên thời gian sống của particle
                var currentColor = (Vector4)(particle.color * (particle.lifeTime / particle.maxLifeTime));
                currentColor.w = particle.lifeTime / particle.maxLifeTime;
                colors.Add(currentColor);

                // Thêm scale của từng particle vào danh sách scales
                scales.Add(0.5f); // Sử dụng scale của particle, có thể điều chỉnh tùy ý

                // Khi đủ batchSize hoặc là particle cuối cùng, ta render và reset danh sách
                if (matrices.Count == batchSize || i == _particles.Count - 1)
                {
                    _propertyBlock.SetVectorArray(ColorId, colors);
                    _propertyBlock.SetFloatArray(Scale, scales); // Thiết lập _Scale như một mảng

                    Graphics.DrawMeshInstanced(_particleMesh, 0, _particleMaterial, matrices, _propertyBlock);

                    matrices.Clear();
                    colors.Clear();
                    scales.Clear();
                }
            }
        }
    }
}