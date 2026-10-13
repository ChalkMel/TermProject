using UnityEngine;
using _Source.Resources;

namespace _Source.EnemySystem
{
    public class EnemyRuntime : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyType;
        
        private EnemyPath _path;
        private int _currentPointIndex = 0;
        private float _currentHealth;
        private Credits _credits;

        public EnemyConfig Config => enemyType;
        public float CurrentHealth => _currentHealth;
        public bool IsAlive => _currentHealth > 0;

        public System.Action<EnemyRuntime> OnEnemyDied;
        public System.Action<EnemyRuntime> OnEnemyReachedBase;

        public void Initialize(EnemyConfig config, EnemyPath path, Credits credits)
        {
            enemyType = config;
            _path = path;
            _currentHealth = config.MaxHealth;
            _currentPointIndex = 0;
            _credits = credits;
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
            
            float moveSpeed = enemyType.Speed * Time.deltaTime;
            transform.position += (Vector3)(direction * moveSpeed);
            
            if (Vector2.Distance(transform.position, targetPoint) < 0.001f)
            {
                _currentPointIndex++;
                
                if (_currentPointIndex >= _path.PointCount - 1)
                {
                    ReachBase();
                }
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnEnemyDied?.Invoke(this);
            //TODO money
            
            Destroy(gameObject);
        }

        private void ReachBase()
        {
            OnEnemyReachedBase?.Invoke(this);
            Destroy(gameObject);
        }
    }
}