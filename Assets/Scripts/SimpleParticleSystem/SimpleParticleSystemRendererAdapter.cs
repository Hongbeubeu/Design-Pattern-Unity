using UnityEngine;

namespace SimpleParticleSystem
{
    public class SimpleParticleSystemRendererAdapter : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
            SimpleParticleSystemRenderer.Awake();
        }

        private void Update()
        {
            SimpleParticleSystemRenderer.Update();
        }
    }
}