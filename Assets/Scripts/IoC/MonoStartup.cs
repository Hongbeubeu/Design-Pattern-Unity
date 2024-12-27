using UnityEngine;

namespace IoC
{
    public class MonoStartup : MonoBehaviour, IStartup
    {
        [SerializeField]
        private MonoContext _context;

        private IContext Context => _context;

        public IResolver Resolver => Context.Container;


        public virtual void Start()
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