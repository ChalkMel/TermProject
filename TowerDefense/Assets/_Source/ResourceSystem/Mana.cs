using System;

namespace ResourceSystem
{
    public class Mana
    {
        private float _current;
        private readonly float _max;

        public Mana(float max = 100f, float start = 0f)
        {
            _max = max;
            _current = Math.Clamp(start, 0f, max);
        }

        public event Action<float, float> ManaChanged;

        public float Current => _current;
        public float Max => _max;
        public float Normalized => _max > 0f ? _current / _max : 0f;

        public void Add(float amount)
        {
            if (amount <= 0f) return;
            _current = Math.Min(_current + amount, _max);
            ManaChanged?.Invoke(_current, _max);
        }

        public bool TrySpend(float amount)
        {
            if (amount <= 0f) return true;
            if (_current < amount) return false;
            _current -= amount;
            ManaChanged?.Invoke(_current, _max);
            return true;
        }

        public bool Has(float amount) => _current >= amount;
    }
}