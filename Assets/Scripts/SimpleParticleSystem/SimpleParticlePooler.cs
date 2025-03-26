using System.Collections.Generic;

namespace hcore.SimpleParticleSystem
{
    public static class SimpleParticlePooler
    {
        private const int INITIAL_SIZE = 50;
        private static readonly List<SimpleParticle> Particles = new();

        public static int Count => Particles.Count;

        private static void CreateParticles(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var particle = new SimpleParticle();
                Particles.Add(particle);
            }
        }

        public static SimpleParticle GetParticle()
        {
            if (Particles.Count == 0)
            {
                CreateParticles(INITIAL_SIZE);
            }

            var particle = Particles[0];
            Particles.RemoveAt(0);
            return particle;
        }

        public static void ReturnParticle(SimpleParticle particle)
        {
            Particles.Add(particle);
        }
    }
}