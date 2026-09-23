using System.Collections.Generic;
using UnityEngine;
using _Source.Resources;

namespace _Source.EnemySystem
{
    public class EnemyRuntime : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyType;

        private EnemyPath _path;
        private Base _base;
        private int _currentPointIndex = 0;
        private float _currentHealth;
        private Credits _credits;
        
        private readonly List<float> _activeSlows = new();
        private float _slowMultiplier = 1f;
        public float SlowMultiplier => _slowMultiplier;
        
        private float _attackTimer;
        private bool _reachedBase;

        public EnemyConfig Config => enemyType;
        public float CurrentHealth => _currentHealth;
        public bool IsAlive => _currentHealth > 0;

        public System.Action<EnemyRuntime> OnEnemyDied;
        public System.Action<EnemyRuntime> OnEnemyReachedBase;

        public void Initialize(EnemyConfig config, EnemyPath path, Credits credits, Base baseRef)
        {
            enemyType = config;
            _path = path;
            _currentHealth = config.MaxHealth;
            _currentPointIndex = 0;
            _credits = credits;
            _base = baseRef;

            _activeSlows.Clear();
            _slowMultiplier = 1f;
            _reachedBase = false;
        }

        private void Update()
        {
            if (!IsAlive) return;

            Move();
        }

        private void Move()
        {
            if (_path == null || _path.PointCount < 2) return;

            Vector2 targetPoint = _path.GetNextPoint(_currentPointIndex);
            Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;

            float moveSpeed = enemyType.Speed * _slowMultiplier * Time.deltaTime;
            transform.position += (Vector3)(direction * moveSpeed);

            if (Vector2.Distance(transform.position, targetPoint) < 0.01f)
            {
                _currentPointIndex++;

                if (_currentPointIndex >= _path.PointCount - 1)
                    ReachBase();
            }
        }
        
        public void AddSlow(float slowFraction)
        {
            _activeSlows.Add(Mathf.Clamp01(slowFraction));
            RecalculateSlow();
        }
        
        public void RemoveSlow(float slowFraction)
        {
            _activeSlows.Remove(Mathf.Clamp01(slowFraction));
            RecalculateSlow();
        }

        private void RecalculateSlow()
        {
            float strongest = 0f;
            for (int i = 0; i < _activeSlows.Count; i++)
                if (_activeSlows[i] > strongest)
                    strongest = _activeSlows[i];

            _slowMultiplier = 1f - strongest;
        }

        private void ReachBase()
        {
            _reachedBase = true;
            _base.GetDamage(enemyType.Damage);
            OnEnemyReachedBase?.Invoke(this);
            Destroy(gameObject);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            OnEnemyDied?.Invoke(this);
            _credits.AddMoney(enemyType.Reward);
            Destroy(gameObject);
        }
    }
}