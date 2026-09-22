using System.Collections.Generic;
using UnityEngine;
using _Source.EnemySystem;

namespace _Source.TowersSystem
{
    public class TowerRuntime : MonoBehaviour
    {
        [SerializeField] private TowerConfig tower;
        
        private float _currentCooldown = 0f;
        private List<EnemyRuntime> _enemiesInRange = new List<EnemyRuntime>();
        private EnemyRuntime _currentTarget;
        public int Level = 1;
        public int Damage
        {
            get => tower.Damage;
            set => tower.Damage = value;
        }

        public TowerConfig Config => tower;
        public float Range
        {
            get => tower.Range;
            set => tower.Range = value;
        }

        public float CurrentCooldown => _currentCooldown;
        public int GetUpgradeCost() => Config.Cost * Level;
        public int GetSellPrice() => (Config.Cost * Level) / 4;
        public Vector3Int CellPosition { get; private set; }
        public bool IsReadyToShoot => _currentCooldown <= 0;

        public System.Action<EnemyRuntime> OnEnemyTargeted;

        public void Initialize(TowerConfig config, Vector3Int cellPosition)
        {
            tower = config;
            CellPosition = cellPosition;
            name = $"Tower_{config.Name}";
        }

        private void Update()
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= Time.deltaTime;
            }
            
            UpdateTarget();
            
            if (IsReadyToShoot && _currentTarget != null)
            {
                Shoot();
            }
        }

        private void UpdateTarget()
        {
            _enemiesInRange.RemoveAll(e => e == null || !e.IsAlive);
            
            if (_currentTarget == null || !_currentTarget.IsAlive || !IsEnemyInRange(_currentTarget))
            {
                _currentTarget = FindClosestEnemy();
                if (_currentTarget != null)
                {
                    OnEnemyTargeted?.Invoke(_currentTarget);
                }
            }
        }

        private EnemyRuntime FindClosestEnemy()
        {
            EnemyRuntime closest = null;
            float closestDistance = float.MaxValue;
            
            foreach (var enemy in _enemiesInRange)
            {
                if (enemy == null || !enemy.IsAlive) continue;
                
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance && distance <= tower.Range)
                {
                    closestDistance = distance;
                    closest = enemy;
                }
            }
            
            return closest;
        }

        private bool IsEnemyInRange(EnemyRuntime enemy)
        {
            if (enemy == null) return false;
            return Vector2.Distance(transform.position, enemy.transform.position) <= tower.Range;
        }

        public void OnEnemyEnteredRange(EnemyRuntime enemy)
        {
            if (enemy != null && !_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Add(enemy);
            }
        }

        public void OnEnemyExitedRange(EnemyRuntime enemy)
        {
            _enemiesInRange.Remove(enemy);
        }

        private void Shoot()
        {
            if (_currentTarget == null) return;
            _currentTarget.TakeDamage(Damage);
            Debug.Log($"{tower.name} damaged {Damage} damage {_currentTarget.name}");
            _currentCooldown = tower.Cooldown;
        }

        private void OnDrawGizmos()
        {
            if (tower != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, tower.Range);
            }
        }
    }
}