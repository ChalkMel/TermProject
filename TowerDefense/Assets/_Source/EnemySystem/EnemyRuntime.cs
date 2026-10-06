using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using BaseSystem;
using EnemySystem.WavesSystem.Config;

namespace EnemySystem
{
    public class EnemyRuntime : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyType;
        [SerializeField] private EnemyHealthBar healthBar;

        [Header("Death")]
        [SerializeField] private ParticleSystem deathParticles;
        [Tooltip("Через сколько секунд после смерти удалять объект. " +
                 "Должно быть >= длительности партикла, иначе он не доиграет.")]
        [SerializeField] private float deathDelay = 1f;
        [SerializeField] private bool autoDelayFromParticles = false;
        [SerializeField] private Collider2D bodyCollider;
        [SerializeField] private SpriteRenderer bodyRenderer;

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
        private bool _isDying;

        public EnemyConfig Config => enemyType;
        public float CurrentHealth => _currentHealth;
        public bool IsAlive => !_isDying && _currentHealth > 0;

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
            _isDying = false;

            if (healthBar == null) healthBar = GetComponentInChildren<EnemyHealthBar>(true);
            if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
            if (bodyRenderer == null) bodyRenderer = GetComponentInChildren<SpriteRenderer>();

            if (healthBar != null) healthBar.SetValue(1f);
            if (bodyCollider != null) bodyCollider.enabled = true;
            if (bodyRenderer != null) bodyRenderer.enabled = true;
        }

        private void Update()
        {
            if (_isDying) return;
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

            if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
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
            if (_isDying) return;
            _reachedBase = true;

            _base.GetDamage(enemyType.Damage);
            OnEnemyReachedBase?.Invoke(this);
            Destroy(gameObject);
        }

        public void TakeDamage(int damage)
        {
            if (_isDying) return;

            _currentHealth -= damage;
            if (healthBar != null)
                healthBar.SetValue(enemyType.MaxHealth > 0
                    ? _currentHealth / enemyType.MaxHealth
                    : 0f);

            if (_currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            if (_isDying) return;
            _isDying = true;
            
            OnEnemyDied?.Invoke(this);
            _credits.AddMoney(enemyType.Reward);
            
            if (bodyCollider != null) bodyCollider.enabled = false;
            if (healthBar != null) healthBar.Hide();
            
            float delay = deathDelay;
            if (deathParticles != null)
            {
                deathParticles.Play();
                if (autoDelayFromParticles)
                {
                    var main = deathParticles.main;
                    delay = main.duration + main.startLifetime.constantMax;
                }
            }
            
            StartCoroutine(DeathRoutine(delay));
        }

        private IEnumerator DeathRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}