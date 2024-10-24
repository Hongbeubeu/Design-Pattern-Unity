using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SimpleParticleSystem
{
    public class SimpleParticleSystem : MonoBehaviour
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        public int particleCount = 100;
        public float emissionRate = 5f;
        public float particleLifeTime = 3f;
        public Color particleColor = Color.white;

        private List<SimpleParticle> _particles = new();
        private Mesh _particleMesh;
        private Material _particleMaterial;

        private void Start()
        {
            _particleMesh = CreateQuadMesh();
            _particleMaterial = new Material(Shader.Find("Unlit/Color"));
        }

        private void Update()
        {
            EmitParticles();
            UpdateParticles();
            RenderParticles();
        }

        private Mesh CreateQuadMesh()
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
            if (_particles.Count < particleCount)
            {
                for (var i = 0; i < emissionRate; i++)
                {
                    var position = transform.position;
                    var velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), 0f);
                    _particles.Add(new SimpleParticle(position, velocity, particleLifeTime, particleColor));
                }
            }
        }

        private void UpdateParticles()
        {
            for (var i = _particles.Count - 1; i > 0; i--)
            {
                var particle = _particles[i];
                particle.lifeTime -= Time.deltaTime;
                if (particle.IsAlive)
                {
                    particle.position += particle.velocity * Time.deltaTime;
                }
                else
                {
                    _particles.RemoveAt(i); // Remove dead particle
                }
            }
        }

        private void RenderParticles()
        {
            foreach (var particle in _particles)
            {
                var matrix = Matrix4x4.TRS(particle.position, Quaternion.identity, Vector3.one * 0.1f);
                _particleMaterial.SetColor(ColorId, particle.color * (particle.lifeTime / particle.maxLifeTime));
                Graphics.DrawMesh(_particleMesh, matrix, _particleMaterial, 0);
            }
        }
    }
}