using UnityEngine;

namespace IoC
{
    public class MonoStartup : MonoBehaviour, IStartup
    {
        [SerializeField] private MonoContext context;
        private IContext Context => context;

        public IResolver Resolver => Context.Container;


        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            Context.Bind();
        }

        public void ResetGame()
        {
            Context.Unbind();
            StartGame();
        }
    }
}