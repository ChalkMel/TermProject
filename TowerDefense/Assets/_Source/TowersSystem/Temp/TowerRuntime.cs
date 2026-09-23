using System;
using System.Collections.Generic;
using UnityEngine;
using _Source.EnemySystem;

namespace _Source.TowersSystem
{
    public class TowerRuntime : MonoBehaviour
    {
        [SerializeField] private TowerConfig tower;

        public TowerConfig Config => tower;
        public Vector3Int CellPosition { get; private set; }
        private Action _onBuffValueChanged;
        
        public int Level { get; private set; } = 1;

        private int _levelDamageBonus;
        private float _levelRangeBonus;
        
        private readonly List<TowerRuntime> _buffers = new();
        private float _damageBuffPercent;
        
        private float _currentCooldown;
        private readonly List<EnemyRuntime> _enemiesInRange = new();
        private EnemyRuntime _currentTarget;

        public bool IsBuffer => tower.Type == TowerType.Buffer;

        public int Damage
        {
            get
            {
                int baseDamage = tower.Damage + _levelDamageBonus;
                return Mathf.RoundToInt(baseDamage * (1f + _damageBuffPercent / 100f));
            }
        }

        public float Range => tower.Range + _levelRangeBonus;
        public float Cooldown => tower.Cooldown;
        public bool IsReadyToShoot => _currentCooldown <= 0f;

        public Action<EnemyRuntime> OnEnemyTargeted;

        public void Initialize(TowerConfig config, Vector3Int cellPosition)
        {
            tower = config;
            CellPosition = cellPosition;
            name = $"Tower_{config.Name}";
            _currentCooldown = 0f;
            _onBuffValueChanged += RecalculateBuffs;
        }

        private void Update()
        {
            if (IsBuffer) return;

            if (_currentCooldown > 0f)
                _currentCooldown -= Time.deltaTime;

            UpdateTarget();

            if (IsReadyToShoot && _currentTarget != null)
                Shoot();
        }
        

        public void OnTowerEnteredRange(TowerRuntime bufferTower)
        {
            if (bufferTower == null || bufferTower == this) return;
            if (!bufferTower.IsBuffer) return;
            if (_buffers.Contains(bufferTower)) return;

            _buffers.Add(bufferTower);
            RecalculateBuffs();
        }

        public void OnTowerExitedRange(TowerRuntime bufferTower)
        {
            if (bufferTower == null) return;
            if (_buffers.Remove(bufferTower))
                RecalculateBuffs();
        }

        private void RecalculateBuffs()
        {
            float bestPercent = 0f;

            for (int i = _buffers.Count - 1; i >= 0; i--)
            {
                var b = _buffers[i];
                if (b == null) { _buffers.RemoveAt(i); continue; }
                
                if (!b.IsBuffer) continue;

                if (b.Config.Buff > bestPercent)
                    bestPercent = b.Config.Buff * b.Level;
            }

            _damageBuffPercent = bestPercent;
        }
        

        private void UpdateTarget()
        {
            _enemiesInRange.RemoveAll(e => e == null || !e.IsAlive);

            bool targetInvalid =
                _currentTarget == null ||
                !_currentTarget.IsAlive ||
                !IsEnemyInRange(_currentTarget);

            if (targetInvalid)
            {
                _currentTarget = FindClosestEnemy();
                if (_currentTarget != null)
                    OnEnemyTargeted?.Invoke(_currentTarget);
            }
        }

        private EnemyRuntime FindClosestEnemy()
        {
            EnemyRuntime closest = null;
            float closestSqr = float.MaxValue;
            float rangeSqr = Range * Range;

            foreach (var enemy in _enemiesInRange)
            {
                if (enemy == null || !enemy.IsAlive) continue;

                float sqr = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sqr <= rangeSqr && sqr < closestSqr)
                {
                    closestSqr = sqr;
                    closest = enemy;
                }
            }
            return closest;
        }

        private bool IsEnemyInRange(EnemyRuntime enemy)
        {
            if (enemy == null) return false;
            return ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude
                   <= Range * Range;
        }

        public void OnEnemyEnteredRange(EnemyRuntime enemy)
        {
            if (enemy != null && !_enemiesInRange.Contains(enemy))
                _enemiesInRange.Add(enemy);
        }

        public void OnEnemyExitedRange(EnemyRuntime enemy)
        {
            _enemiesInRange.Remove(enemy);
        }

        private void Shoot()
        {
            if (_currentTarget == null) return;
            _currentTarget.TakeDamage(Damage);
            Debug.Log($"{name} dealt {Damage} damage to {_currentTarget.name}");
            _currentCooldown = Cooldown;
        }

        public int GetUpgradeCost() => Config.Cost * Level;
        public int GetSellPrice() => Mathf.Max(1, (Config.Cost * Level) / 2);

        public void Upgrade()
        {
            Level++;
            if (!IsBuffer)
            {
                _levelDamageBonus += 1;
                _levelRangeBonus += 0.5f;
            }
            else if(IsBuffer)
            {
                _onBuffValueChanged?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            if (tower == null) return;
            Gizmos.color = IsBuffer ? Color.cyan : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Range);
        }

        private void OnDestroy()
        {
            _onBuffValueChanged -= RecalculateBuffs;
        }
    }
}