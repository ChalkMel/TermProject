using System.Collections.Generic;
using Pool;

public class ObjectPool<T> : IObjectPool<T> where T : IPoolableItem
{
    private readonly Queue<T> _poolableObjects;
    private readonly IFactory<T> _factory;
    private readonly int _maxPoolSize;
    private int _totalCreatedCount;

    public ObjectPool(IFactory<T> factory, int maxPoolSize = 8, int startPoolSize = 3)
    {
        _factory       = factory;
        _maxPoolSize   = maxPoolSize;
        _poolableObjects = new Queue<T>();
        _totalCreatedCount = 0;

        for (int i = 0; i < startPoolSize; i++)
        {
            T item = _factory.Create();
            item.Deactivate();
            _poolableObjects.Enqueue(item);
            _totalCreatedCount++;
        }
    }

    public bool TryGet(out T item)
    {
        item = default;

        if (_poolableObjects.Count > 0)
            item = _poolableObjects.Dequeue();
        else if (_totalCreatedCount < _maxPoolSize)
        {
            item = _factory.Create();
            _totalCreatedCount++;
        }
        else return false;

        var poolableItem = item;
        item.SetCallback(() => Return(poolableItem));
        item.Activate();
        return true;
    }

    public void Return(T poolableObject)
    {
        if (poolableObject == null) return;
        poolableObject.Deactivate();
        _poolableObjects.Enqueue(poolableObject);
    }
}