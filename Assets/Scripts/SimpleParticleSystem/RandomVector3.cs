using System;

namespace hcore.SimpleParticleSystem
{
    [Serializable]
    public class RandomVector3
    {
        public RandomNumberFloat randomX;
        public RandomNumberFloat randomY;
        public RandomNumberFloat randomZ;

        public UnityEngine.Vector3 GetValue()
        {
            return new UnityEngine.Vector3(randomX.GetValue(), randomY.GetValue(), randomZ.GetValue());
        }
    }
}