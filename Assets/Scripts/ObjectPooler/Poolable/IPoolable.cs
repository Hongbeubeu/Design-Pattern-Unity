namespace ObjectPooler
{
    public interface IPoolable
    {
        Pool Pool { get; set; } // The pool that the object belongs to

        void OnSpawn();

        // Called when the object is retrieved from the pool
        void OnReturn();

        // Called when the object is returned to the pool
        void ReturnToPool(); // Returns the object to the pool
    }
}