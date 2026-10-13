using System.Collections.Generic;
using Core.Effects;
using UnityEngine;

namespace EnemySystem.Status
{ 
    public sealed class EnemyStatusController : MonoBehaviour
    {
        private readonly ModifierSet _slowPercent = new(ModifierSet.Combine.Max);

        private readonly Dictionary<EnemyStat, ModifierSet> _multipliers = new()
        {
            { EnemyStat.MoveSpeed,     new ModifierSet(ModifierSet.Combine.Product) },
            { EnemyStat.OutgoingDamage, new ModifierSet(ModifierSet.Combine.Product) }
        };
        
        public bool IsSlowed  => !_slowPercent.IsEmpty;
        
        public float MoveSpeedMultiplier =>
            (1f - _slowPercent.Resolve()) * Resolve(EnemyStat.MoveSpeed);
        
        public float DamageMultiplier => Resolve(EnemyStat.OutgoingDamage);

        private float Resolve(EnemyStat stat) =>
            _multipliers.TryGetValue(stat, out var set) ? set.Resolve() : 1f;
        
        public void AddSlow(object source, float fraction) =>
            _slowPercent.Set(source, Mathf.Clamp01(fraction));

        public void RemoveSlow(object source) => _slowPercent.Remove(source);
        
        public void AddMultiplier(object source, EnemyStat stat, float multiplier)
        {
            if (_multipliers.TryGetValue(stat, out var set)) set.Set(source, multiplier);
        }

        public void RemoveMultiplier(object source, EnemyStat stat)
        {
            if (_multipliers.TryGetValue(stat, out var set)) set.Remove(source);
        }
    }
}