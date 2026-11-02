using System.Collections.Generic;
using UnityEngine;
using TowersSystem.Stats;

namespace TowersSystem.Runtimes
{
    public class BufferTowerRuntime : TowerRuntimeBase
    {
        [Header("Upgrade Bonus")]
        private const int buff_bonus_per_level = 5;
        private const float range_bonus_per_level = 0.2f;

        private readonly List<AttackerTowerRuntime> _buffedAttackers = new();
        private int _levelBuffBonus;

        public int EffectiveBuff => (int)(Config.Buff + _levelBuffBonus);

        protected override Color GizmoColor => Color.cyan;

        public override void UpgradeTower()
        {
            base.UpgradeTower();
            _levelBuffBonus += buff_bonus_per_level;
            _levelRangeBonus += range_bonus_per_level;
            RefreshBuffs();
        }

        public override void OnTowerEnteredRange(TowerRuntimeBase other)
        {
            if (other is AttackerTowerRuntime attacker)
            {
                ApplyBuff(attacker);
                if (!_buffedAttackers.Contains(attacker))
                    _buffedAttackers.Add(attacker);
            }
        }

        public override void OnTowerExitedRange(TowerRuntimeBase other)
        {
            if (other is AttackerTowerRuntime attacker)
            {
                RemoveBuff(attacker);
                _buffedAttackers.Remove(attacker);
            }
        }

        private void ApplyBuff(AttackerTowerRuntime attacker) =>
            attacker.Status.AddMultiplier(this, TowerStat.OutgoingDamage, 1f + EffectiveBuff / 100f);

        private void RemoveBuff(AttackerTowerRuntime attacker) =>
            attacker.Status.RemoveMultiplier(this, TowerStat.OutgoingDamage);

        private void RefreshBuffs()
        {
            for (int i = _buffedAttackers.Count - 1; i >= 0; i--)
            {
                var a = _buffedAttackers[i];
                if (a == null) { _buffedAttackers.RemoveAt(i); continue; }
                ApplyBuff(a);
            }
        }

        public override void OnBeforeDestroy()
        {
            foreach (var a in _buffedAttackers)
                if (a != null) RemoveBuff(a);
            _buffedAttackers.Clear();
        }

        public override string GetTooltipText()
        {
            return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
                   $"Type: Buffer\n" +
                   $"Damage Buff: +{EffectiveBuff}%\n" +
                   $"Range: {Range:F1}";
        }
        
        public override string GetUpgradePreviewText()
        {
            int nextBuff    = EffectiveBuff + buff_bonus_per_level;
            float nextRange = Range + level_range_bonus + range_bonus_per_level;

            return $"Buff: +{EffectiveBuff}% → +{nextBuff}%\n" +
                   $"Range: {Range:F1} → {nextRange:F1}";
        }
    }
}