using UnityEngine;

namespace BaseSystem
{
    public class Base : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;

        private float _currentHealth;

        public float Health => _currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => _currentHealth > 0f;

        public System.Action<float, float> OnHealthChanged;
        public System.Action OnBaseDestroyed;

        public void ConfigureMaxHealth(float value)
        {
            if (value <= 0f) return;
            maxHealth = value;
            _currentHealth = value;
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }

        public void GetDamage(float damage)
        {
            if (!IsAlive || damage <= 0f) return;

            _currentHealth -= damage;
            if (_currentHealth < 0f) _currentHealth = 0f;

            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

            if (_currentHealth <= 0f) OnBaseDestroyed?.Invoke();
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f) return;

            _currentHealth += amount;
            if (_currentHealth > maxHealth) _currentHealth = maxHealth;

            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }
    }
}