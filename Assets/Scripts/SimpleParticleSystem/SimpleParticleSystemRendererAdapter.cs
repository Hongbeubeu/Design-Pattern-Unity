using UnityEngine;

namespace hcore.SimpleParticleSystem
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