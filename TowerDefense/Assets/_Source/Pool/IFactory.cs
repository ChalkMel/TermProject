using UnityEngine;

public interface IFactory<T>
{
    T Create();
    T Create(Vector3 position, Quaternion rotation);
}