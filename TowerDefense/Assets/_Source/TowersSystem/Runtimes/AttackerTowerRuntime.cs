using System.Collections.Generic;
using _Source.EnemySystem;
using UnityEngine;
using _Source.TowersSystem;

namespace _Source.TowersSystem.Runtimes
{
    public class AttackerTowerRuntime : TowerRuntimeBase, IBuffReceiver
    {
        private float _currentCooldown;
        private readonly List<EnemyRuntime> _enemiesInRange = new();
        private EnemyRuntime _currentTarget;

        private readonly List<BufferTowerRuntime> _buffers = new();
        private float _damageBuffPercent;
        private int _levelDamageBonus;
        private IBuffReceiver _buffReceiverImplementation;

        public int Damage =>
            Mathf.RoundToInt((tower.Damage + _levelDamageBonus) * (1f + _damageBuffPercent / 100f));

        public bool IsReadyToShoot => _currentCooldown <= 0f;

        protected override Color GizmoColor => Color.yellow;

        public override void Upgrade()
        {
            base.Upgrade();
            _levelDamageBonus += 1;
        }

        private void Update()
        {
            if (_currentCooldown > 0f) _currentCooldown -= Time.deltaTime;

            UpdateTarget();
            if (IsReadyToShoot && _currentTarget != null) Shoot();
        }

        public override void OnEnemyEnteredRange(EnemyRuntime enemy)
        {
            if (enemy != null && !_enemiesInRange.Contains(enemy))
                _enemiesInRange.Add(enemy);
        }

        public override void OnEnemyExitedRange(EnemyRuntime enemy)
        {
            _enemiesInRange.Remove(enemy);
        }
        public void RegisterBuffer(BufferTowerRuntime buffer)
        {
            if (buffer == null || _buffers.Contains(buffer)) return;
            _buffers.Add(buffer);
            RecalculateBuffs();
        }
        public void UnregisterBuffer(BufferTowerRuntime buffer) { }


        public void OnBufferValueChanged() => RecalculateBuffs();

        private void RecalculateBuffs()
        {
            float best = 0f;
            for (int i = _buffers.Count - 1; i >= 0; i--)
            {
                var b = _buffers[i];
                if (b == null) { _buffers.RemoveAt(i); continue; }
                if (b.Config.Buff > best)
                    best = b.Config.Buff;
            }
            _damageBuffPercent = best;
        }
        
        private void UpdateTarget()
        {
            _enemiesInRange.RemoveAll(e => e == null || !e.IsAlive);

            bool targetInvalid =
                _currentTarget == null ||
                !_currentTarget.IsAlive ||
                !IsEnemyInRange(_currentTarget);

            if (targetInvalid)
                _currentTarget = FindClosestEnemy();
        }

        private EnemyRuntime FindClosestEnemy()
        {
            EnemyRuntime closest = null;
            float closestSqr = float.MaxValue;
            float rangeSqr = Range * Range;

            foreach (var e in _enemiesInRange)
            {
                if (e == null || !e.IsAlive) continue;
                float sqr = ((Vector2)e.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sqr <= rangeSqr && sqr < closestSqr) { closestSqr = sqr; closest = e; }
            }
            return closest;
        }

        private bool IsEnemyInRange(EnemyRuntime enemy)
        {
            if (enemy == null) return false;
            return ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude
                   <= Range * Range;
        }

        private void Shoot()
        {
            _currentTarget.TakeDamage(Damage);
            _currentCooldown = tower.Cooldown;
        }

        public override void OnBeforeDestroy()
        {
            _enemiesInRange.Clear();
            _buffers.Clear();
        }

        public override string GetTooltipText()
        {
            var baseText = base.GetTooltipText();
            string buffLine = _damageBuffPercent > 0f
                ? $"\nBuff: +{_damageBuffPercent}%"
                : "";
            return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
                   $"Damage: {Damage}\n" +
                   $"Range: {Range:F1}\n" +
                   $"Cooldown: {Config.Cooldown}s{buffLine}";
        }
    }
}