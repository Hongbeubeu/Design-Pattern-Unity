using UnityEngine;

namespace ObjectPooler
{
    public abstract class PoolableObjectBase : IPoolable
    {
        #region Poolable

        public Pool Pool { get; set; }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnReturn()
        {
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