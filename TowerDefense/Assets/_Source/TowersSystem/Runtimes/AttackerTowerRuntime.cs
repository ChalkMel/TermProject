using System.Collections.Generic;
using UnityEngine;
using EnemySystem;

namespace TowersSystem.Runtimes
{
    public class AttackerTowerRuntime : TowerRuntimeBase
    {
        private float _currentCooldown;
        private readonly List<EnemyRuntime> _enemiesInRange = new();
        private EnemyRuntime _currentTarget;
        private float _levelDamageBonus;
        private int maxLevel = 7;
        

        public float Damage =>
            (Tower.Damage + _levelDamageBonus) * Status.DamageMultiplier;

        public bool IsReadyToShoot => _currentCooldown <= 0f;

        protected override Color GizmoColor => Color.yellow;

        public override bool UpgradeTower()
        {
            if (!base.UpgradeTower()) return false;
            _levelDamageBonus += Config.Damage * 0.3f;
            return true;
        }

        private void Update()
        {
            if (_currentCooldown > 0f)
                _currentCooldown -= Time.deltaTime * Status.AttackSpeedMultiplier;

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

        public EnemyRuntime FindClosestEnemy()
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

        public void UpdateCooldown() => _currentCooldown = 0;

        private bool IsEnemyInRange(EnemyRuntime enemy)
        {
            if (enemy == null) return false;
            return ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude
                   <= Range * Range;
        }

        protected virtual float StunOnHit => 0f;

        protected virtual void Shoot()
        {
            if (_currentTarget == null) return;
            var go = Instantiate(Config.ProjectilePrefab, transform.position, Quaternion.identity);
            var proj = go.GetComponent<Projectile>();
            if (proj != null)
                proj.Initialize(_currentTarget, Damage, Config.ProjectileSpeed, this, StunOnHit);

            _currentCooldown = Config.Cooldown;
        }

        public override void OnBeforeDestroy()
        {
            _enemiesInRange.Clear();
        }

        public override string GetTooltipText()
        {
            float buffPercent = (Status.DamageMultiplier - 1f) * 100f;
            string buffLine = Mathf.Abs(buffPercent) > 0.01f
                ? $"\nBuff: {(buffPercent >= 0 ? "+" : "")}{buffPercent:F0}%"
                : "";

            return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
                   $"Damage: {Damage}\n" +
                   $"Range: {Range:F1}\n" +
                   $"Cooldown: {Config.Cooldown * Status.AttackSpeedMultiplier}s{buffLine}";
        }
        public override string GetUpgradePreviewText()
        {
            float nextBonus  = _levelDamageBonus + Config.Damage * 0.3f;
            float nextDamage = (Config.Damage + nextBonus) * Status.DamageMultiplier;
            float nextRange  = Range + level_range_bonus;

            return $"Damage:\n {Damage:F1} → <color={color}>{nextDamage:F1}\n</color={color}>" +
                   $"Range:\n {Range:F1} → <color={color}>{nextRange:F1}</color={color}>";
        }
    }
}