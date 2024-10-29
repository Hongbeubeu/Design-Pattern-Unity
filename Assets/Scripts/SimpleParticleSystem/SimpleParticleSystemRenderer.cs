using System.Collections.Generic;
using UnityEngine;

namespace SimpleParticleSystem
{
    public static class SimpleParticleSystemRenderer
    {
        private static readonly Dictionary<Mesh, List<SimpleParticleSystem>> ParticleSystemDictionary = new();
        private static readonly List<Matrix4x4> Matrices = new();
        private static readonly List<Vector4> Colors = new();
        private static MaterialPropertyBlock _propertyBlock;
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static Mesh _defaultMesh;

        public static void Awake()
        {
            _defaultMesh = CreateQuadMesh();
            ParticleSystemDictionary.Clear();
        }

        public static void AddParticleSystem(SimpleParticleSystem particleSystem)
        {
            if (particleSystem.particleMesh == null)
            {
                particleSystem.particleMesh = _defaultMesh;
            }

            if (!ParticleSystemDictionary.ContainsKey(particleSystem.particleMesh))
            {
                ParticleSystemDictionary.Add(particleSystem.particleMesh, new List<SimpleParticleSystem>());
            }

            ParticleSystemDictionary[particleSystem.particleMesh].Add(particleSystem);
        }

        public static void RemoveParticleSystem(SimpleParticleSystem particleSystem)
        {
            if (ParticleSystemDictionary.TryGetValue(particleSystem.particleMesh, out var particleSystems))
            {
                particleSystems.Remove(particleSystem);
            }
        }

        public static void Update()
        {
            foreach (var particleSystem in ParticleSystemDictionary)
            {
                foreach (var p in particleSystem.Value)
                {
                    p.DoUpdate();
                }
            }

            DoRenderer();
        }

        private static void DoRenderer()
        {
            const int batchSize = 511;
            Matrices.Clear();
            Colors.Clear();
            _propertyBlock ??= new MaterialPropertyBlock();
            _propertyBlock.Clear();

            foreach (var (mesh, particleSystems) in ParticleSystemDictionary)
            {
                var particles = new List<SimpleParticle>();
                var material = particleSystems[0].particleMaterial;
                foreach (var particleSystem in particleSystems)
                {
                    particles.AddRange(particleSystem.Particles);
                }

                for (var i = 0; i < particles.Count; i++)
                {
                    var particle = particles[i];

                    // Tạo matrix cho từng particle
                    var matrix = Matrix4x4.TRS(particle.Position, particle.Rotation, particle.Scale);
                    Matrices.Add(matrix);

                    // Thêm màu của particle vào danh sách
                    var currentColor = (Vector4)particle.Color;
                    Colors.Add(currentColor);

                    // Vẽ particles theo batch size
                    if (Matrices.Count == batchSize || i == particles.Count - 1)
                    {
                        // Gán danh sách màu vào MaterialPropertyBlock
                        _propertyBlock.SetVectorArray(ColorId, Colors);

                        // Render batch hiện tại
                        Graphics.RenderMeshInstanced(
                            new RenderParams(material) { matProps = _propertyBlock },
                            mesh,
                            0,
                            Matrices
                        );

                        // Xóa matrices và colors sau mỗi batch
                        Matrices.Clear();
                        Colors.Clear();
                    }
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