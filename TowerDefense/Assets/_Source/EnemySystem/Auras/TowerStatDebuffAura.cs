using System.Collections.Generic;
using TowersSystem.Runtimes;
using TowersSystem.Stats;
using UnityEngine;

namespace EnemySystem.Auras
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class TowerStatDebuffAura : MonoBehaviour
    {
        [SerializeField] private TowerStat targetStat = TowerStat.OutgoingDamage;
        [Tooltip("Множитель: 0.9 = -10%.")]
        [SerializeField] private float multiplier = 0.9f;
        [SerializeField] private float radius = 3f;

        private readonly HashSet<TowerRuntimeBase> _towers = new();
        private CircleCollider2D _collider;
        private EnemyRuntime _owner;

        private void Awake()
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
            foreach (var t in _towers)
                if (t != null) t.Status.RemoveMultiplier(this, targetStat);
            _towers.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var tower = other.GetComponentInParent<TowerRuntimeBase>();
            if (tower == null) return;
            if (_towers.Add(tower))
                tower.Status.AddMultiplier(this, targetStat, multiplier);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var tower = other.GetComponentInParent<TowerRuntimeBase>();
            if (tower == null) return;
            if (_towers.Remove(tower))
                tower.Status.RemoveMultiplier(this, targetStat);
        }
    }
}