using System;
using System.Collections.Concurrent;

namespace TanakaShoji.Discord.Gateway.WebSocket
{
    internal class ObjectPool<T>
    {
        private readonly Func<T> _factory;
        private readonly ConcurrentBag<T> _objects = new ConcurrentBag<T>();

        public ObjectPool(Func<T> factory)
        {
            _factory = factory ?? throw new ArgumentNullException();
        }

        public bool IsEmpty => _objects.IsEmpty;

        public T Get()
        {
            return _objects.TryTake(out var obj) ? obj : _factory();
        }

        public void Put(T obj)
        {
            // FIXME: release unused objects
            _objects.Add(obj);
        }
    }
}