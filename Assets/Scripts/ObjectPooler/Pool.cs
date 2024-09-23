using System;
using System.Collections.Generic;

namespace ObjectPooler
{
    public class Pool
    {
        private readonly Queue<IPoolable> _objectQueue;
        private readonly Func<IPoolable> _createFunc;
        private int _additionalSize;

        public Pool(Func<IPoolable> createFunc, int initialSize, int additionalSize)
        {
            _createFunc = createFunc;
            _additionalSize = additionalSize;
            _objectQueue = new Queue<IPoolable>();
            for (var i = 0; i < initialSize; i++)
            {
                var obj = createFunc();
                obj.Pool = this;
                obj.OnReturn();
                _objectQueue.Enqueue(obj);
            }
        }

        private void AddAdditionalObjects()
        {
            // Ensure that the additional size is at least 1
            if (_additionalSize == 0)
            {
                _additionalSize = 1;
            }

            for (var i = 0; i < _additionalSize; i++)
            {
                var obj = _createFunc();
                obj.Pool = this;
                obj.OnReturn();
                _objectQueue.Enqueue(obj);
            }
        }

        public IPoolable Get()
        {
            if (_objectQueue.Count == 0)
            {
                AddAdditionalObjects();
            }

            var obj = _objectQueue.Dequeue();
            return obj;
        }

        public void Return(IPoolable obj)
        {
            obj.OnReturn();
            _objectQueue.Enqueue(obj);
        }
    }
}