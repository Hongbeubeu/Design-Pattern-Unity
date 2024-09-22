namespace ObjectPooler
{
    public interface IPoolable
    {
        void OnSpawn(); // Called when the object is retrieved from the pool
        void OnReturn(); // Called when the object is returned to the pool
    }
}