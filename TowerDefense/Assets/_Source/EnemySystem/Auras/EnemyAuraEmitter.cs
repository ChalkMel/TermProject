using System.Collections.Generic;
using UnityEngine;

namespace EnemySystem.Auras
{
  [RequireComponent(typeof(CircleCollider2D))]
  public abstract class EnemyAuraEmitter : MonoBehaviour
  {
    [SerializeField] protected float radius = 3f;

    private readonly HashSet<EnemyRuntime> _enemies = new();
    private CircleCollider2D _collider;
    private EnemyRuntime _owner;

    protected virtual void Awake()
    {
      _collider = GetComponent<CircleCollider2D>();
      _collider.isTrigger = true;
      _collider.radius = radius;

      _owner = GetComponentInParent<EnemyRuntime>();
      if (_owner != null) _owner.OnEnemyDied += HandleOwnerDied;
    }

    private void OnDestroy()
    {
      if (_owner != null) _owner.OnEnemyDied -= HandleOwnerDied;
      ClearAll();
    }

    private void HandleOwnerDied(EnemyRuntime _) => ClearAll();

    private void ClearAll()
    {
      foreach (var e in _enemies) if (e != null) OnEnemyExited(e);
      _enemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      var enemy = other.GetComponentInParent<EnemyRuntime>();
      if (enemy == null || enemy == _owner) return;
      if (_enemies.Add(enemy)) OnEnemyEntered(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      var enemy = other.GetComponentInParent<EnemyRuntime>();
      if (enemy == null) return;
      if (_enemies.Remove(enemy)) OnEnemyExited(enemy);
    }

    protected abstract void OnEnemyEntered(EnemyRuntime enemy);
    protected abstract void OnEnemyExited(EnemyRuntime enemy);
  }
}