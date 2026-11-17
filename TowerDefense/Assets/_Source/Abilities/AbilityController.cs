using System.Collections.Generic;
using BaseSystem;
using EnemySystem;
using HeatSystem;
using Resources;
using ResourceSystem;
using UnityEngine;

namespace Abilities
{
    public class AbilityController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Base baseRef;
        [SerializeField] private HeatSystem.HeatSystem heatSystem;

        private Mana _mana;
        
        private readonly Dictionary<AbilityType, AbilityConfig> _configs = new();
        private readonly Dictionary<AbilityType, float> _cooldownUntil = new();
        private Coroutine _dotRoutine;

        public System.Action<AbilityType> OnAbilityActivated;
        public System.Action<AbilityType> OnAbilityFailed;

        public Mana Mana => _mana;
        public IEnumerable<AbilityConfig> AvailableAbilities => _configs.Values;

        public void Configure(Mana manaRef, Base baseRefRef, HeatSystem.HeatSystem heatRef,
                              IReadOnlyList<AbilityConfig> levelAbilities)
        {
            _mana = manaRef;
            baseRef = baseRefRef;
            heatSystem = heatRef;

            _configs.Clear();
            _cooldownUntil.Clear();

            if (levelAbilities == null) return;

            foreach (var cfg in levelAbilities)
            {
                if (cfg == null) continue;
                if (!_configs.ContainsKey(cfg.Type))
                    _configs.Add(cfg.Type, cfg);
            }
        }

        public bool HasAbility(AbilityType type) => _configs.ContainsKey(type);
        public AbilityConfig GetConfig(AbilityType type) =>
            _configs.TryGetValue(type, out var cfg) ? cfg : null;

        public bool IsOnCooldown(AbilityType type) =>
            _cooldownUntil.TryGetValue(type, out float t) && Time.time < t;

        public float CooldownRemaining(AbilityType type) =>
            IsOnCooldown(type) ? _cooldownUntil[type] - Time.time : 0f;

        public bool CanActivate(AbilityType type)
        {
            if (!HasAbility(type)) return false;
            var cfg = _configs[type];
            if (IsOnCooldown(type)) return false;
            if (_mana == null || !_mana.Has(cfg.ManaCost)) return false;
            return true;
        }

        public bool TryActivate(AbilityType type)
        {
            if (!CanActivate(type))
            {
                OnAbilityFailed?.Invoke(type);
                return false;
            }

            var cfg = _configs[type];
            if (!_mana.TrySpend(cfg.ManaCost)) { OnAbilityFailed?.Invoke(type); return false; }

            _cooldownUntil[type] = Time.time + cfg.Cooldown;
            Execute(cfg);
            OnAbilityActivated?.Invoke(type);
            return true;
        }

        private void Execute(AbilityConfig cfg)
        {
            switch (cfg.Type)
            {
                case AbilityType.DamageOverTime: StartDot(cfg); break;
                case AbilityType.Heal:           HealBase(cfg); break;
            }
        }

        private void StartDot(AbilityConfig cfg)
        {
            if (_dotRoutine != null) StopCoroutine(_dotRoutine);
            _dotRoutine = StartCoroutine(DotRoutine(cfg));
        }

        private System.Collections.IEnumerator DotRoutine(AbilityConfig cfg)
        {
            float duration = cfg.DotDuration;
            float tick = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                DealDamageToAllEnemies(cfg.DotDamagePerSecond * tick);
                yield return new WaitForSeconds(tick);
                elapsed += tick;
            }

            _dotRoutine = null;
        }

        private static void DealDamageToAllEnemies(float damage)
        {
            var enemies = FindObjectsByType<EnemyRuntime>(FindObjectsSortMode.None);
            foreach (var e in enemies)
                if (e != null && e.IsAlive) e.TakeDamage(damage);
        }

        private void HealBase(AbilityConfig cfg)
        {
            if (baseRef == null) return;
            baseRef.Heal(cfg.HealAmount);
        }
    }
}