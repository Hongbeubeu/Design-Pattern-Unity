using UnityEngine;

namespace hcore.ObjectPooler
{
    public abstract class PoolableMonoBehaviourBase : MonoBehaviour, IPoolable
    {
        #region Poolable

        public Pool Pool { get; set; }

        public virtual void OnSpawn()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnReturn()
        {
            gameObject.SetActive(false);
        }

        public void ReturnToPool()
        {
            if (Pool == null)
            {
                Debug.LogWarning($"<color=yellow>Cannot return {this} to pool because it does not have a pool.</color>");
                return;
            }

            Pool.Return(this);
        }

        #endregion
    }
}