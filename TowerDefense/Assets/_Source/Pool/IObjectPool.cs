namespace Pool
{
    public interface IObjectPool<T>
    { 
        bool TryGet(out T item); 
        void Return(T poolableObject);
    }
}
