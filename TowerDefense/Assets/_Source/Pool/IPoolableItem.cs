using System;

namespace Pool
{
    public interface IPoolableItem
    {
        void SetCallback(Action callback);
        void Activate();
        void Deactivate();
    }
}