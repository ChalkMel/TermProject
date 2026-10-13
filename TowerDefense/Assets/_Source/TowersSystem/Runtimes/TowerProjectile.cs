using UnityEngine;
using EnemySystem;

namespace TowersSystem.Runtimes
{
  public class Projectile : MonoBehaviour
  {
    [SerializeField] private float hitDistance = 0.15f;
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private float rotateToDirection = 0f;

    private EnemyRuntime _target;
    private int _damage;
    private float _speed;
    private float _lifeTimer;
    private AttackerTowerRuntime _tower;

    public void Initialize(EnemyRuntime target, int damage, float speed, AttackerTowerRuntime attacker)
    {
      _target = target;
      _damage = damage;
      _speed = speed;
      _lifeTimer = maxLifetime;
      _tower = attacker;
    }

    private void Update()
    {
      if (_target == null || !_target.IsAlive)
      {
         _target = _tower.FindClosestEnemy();
         if (_target == null)
         {
           _tower.UpdateCooldown();
           Destroy(gameObject);
           return;
         }
      }

      Vector2 current = transform.position;
      Vector2 goal = _target.transform.position;
      Vector2 direction = (goal - current).normalized;

      transform.position = current + direction * (_speed * Time.deltaTime);

      if (rotateToDirection != 0f)
      {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotateToDirection);
      }

      if (Vector2.Distance(transform.position, goal) <= hitDistance)
      {
        _target.TakeDamage(_damage);
        Destroy(gameObject);
      }
    }
  }
}