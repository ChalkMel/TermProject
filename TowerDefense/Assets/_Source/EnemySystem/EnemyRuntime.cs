using System.Collections;
using UnityEngine;
using Resources;
using BaseSystem;
using EnemySystem.WavesSystem.Config;
using EnemySystem.Status;
using LevelSystem;

namespace EnemySystem
{
  public class EnemyRuntime : MonoBehaviour
  {
    [SerializeField] private EnemyConfig enemyType;
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private float deathDelay = 1f;
    [SerializeField] private bool autoDelayFromParticles = false;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private SpriteRenderer bodyRenderer;

    private EnemyPath _path;
    private Base _base;
    private int _currentPointIndex;
    private Credits _credits;
    
    private float _maxHealth;
    private float _speed;
    private float _damage;
    private int _reward;
    private float _currentHealth;

    private bool _reachedBase;
    private bool _isDying;

    public EnemyStatusController Status { get; private set; }
    public EnemyConfig Config => enemyType;
    public float CurrentHealth => _currentHealth;
    public bool IsAlive => !_isDying && _currentHealth > 0f;
    public float SlowMultiplier => Status != null ? Status.MoveSpeedMultiplier : 1f;

    public System.Action<EnemyRuntime> OnEnemyDied;
    public System.Action<EnemyRuntime> OnEnemyReachedBase;

    private void Awake()
    {
      Status = GetComponent<EnemyStatusController>();
      if (Status == null) Status = gameObject.AddComponent<EnemyStatusController>();
    }

    public void Initialize(
        EnemyConfig config,
        EnemyPath path,
        Credits credits,
        Base baseRef,
        DifficultyConfig difficulty = null)
    {
      enemyType = config;
      _path = path;
      _credits = credits;
      _base = baseRef;

      float hpMult  = difficulty?.EnemyHealthMultiplier ?? 1f;
      float dmgMult = difficulty?.EnemyDamageMultiplier  ?? 1f;
      float spdMult = difficulty?.EnemySpeedMultiplier   ?? 1f;
      float rewMult = difficulty?.EnemyRewardMultiplier  ?? 1f;

      _maxHealth = config.MaxHealth * hpMult;
      _speed     = config.Speed      * spdMult;
      _damage    = config.Damage     * dmgMult;
      _reward    = Mathf.RoundToInt(config.Reward * rewMult);
      _currentHealth = _maxHealth;
      _currentPointIndex = 0;

      _reachedBase = false;
      _isDying = false;

      if (healthBar == null)     healthBar     = GetComponentInChildren<EnemyHealthBar>(true);
      if (bodyCollider == null)  bodyCollider  = GetComponent<Collider2D>();
      if (bodyRenderer == null)  bodyRenderer  = GetComponentInChildren<SpriteRenderer>();

      if (healthBar != null)     healthBar.SetValue(1f);
      if (bodyCollider != null)  bodyCollider.enabled  = true;
      if (bodyRenderer != null)  bodyRenderer.enabled  = true;
    }

    private void Update()
    {
      if (_isDying || !IsAlive) return;
      if (Status != null && Status.IsStunned) return;
      Move();
    }

    private void Move()
    {
      if (_path == null || _path.PointCount < 2) return;

      Vector2 targetPoint = _path.GetNextPoint(_currentPointIndex);
      Vector2 direction   = (targetPoint - (Vector2)transform.position).normalized;
      float moveSpeed     = _speed * Status.MoveSpeedMultiplier * Time.deltaTime;
      transform.position += (Vector3)(direction * moveSpeed);

      if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
      {
        _currentPointIndex++;
        if (_currentPointIndex >= _path.PointCount - 1) ReachBase();
      }
    }

    private void ReachBase()
    {
      if (_isDying) return;
      _reachedBase = true;

      _base.GetDamage(_damage * Status.DamageMultiplier);
      OnEnemyReachedBase?.Invoke(this);
      Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
      if (_isDying) return;

      _currentHealth -= damage;
      if (healthBar != null)
        healthBar.SetValue(_maxHealth > 0f ? _currentHealth / _maxHealth : 0f);

      if (_currentHealth <= 0f) Die();
    }

    private void Die()
    {
      if (_isDying) return;
      _isDying = true;

      OnEnemyDied?.Invoke(this);
      _credits.AddMoney(_reward);

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