using UnityEngine;

public class Factory<T> : IFactory<T> where T : Component
{
    private readonly T _prefab;

    public Factory(T prefab)
    {
        _prefab = prefab;
    }

    public T Create()
    {
        return Object.Instantiate(_prefab);
    }

    public T Create(Vector3 position, Quaternion rotation)
    {
        var instance = Object.Instantiate(_prefab, position, rotation);
        if (instance.TryGetComponent(out T component))
        {
            return component;
        }
        return null; 
    }
}