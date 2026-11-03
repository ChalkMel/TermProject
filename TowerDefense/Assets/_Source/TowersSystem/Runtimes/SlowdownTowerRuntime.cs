using System.Collections.Generic;
using UnityEngine;
using EnemySystem;

namespace TowersSystem.Runtimes
{
    public class SlowdownTowerRuntime : TowerRuntimeBase
    {
        private const int slow_bonus_per_level = 5;
        private const float range_bonus_per_level = .2f;
        private int maxLevel = 7;

        private readonly List<EnemyRuntime> _slowedEnemies = new();
        private int _levelBuffBonus;

        public float EffectiveSlowPercent =>
            Mathf.Min(Config.Buff + _levelBuffBonus);

        protected override Color GizmoColor => new Color(0.4f, 0.7f, 1f);

        public override bool UpgradeTower()
        {
            if (!base.UpgradeTower()) return false;
            _levelBuffBonus += slow_bonus_per_level;
            _levelRangeBonus += range_bonus_per_level;
            RefreshSlows();
            return true;
        }

        public override void OnEnemyEnteredRange(EnemyRuntime enemy)
        {
            if (enemy == null) return;
            enemy.Status.AddSlow(this, EffectiveSlowPercent / 100f);
            if (!_slowedEnemies.Contains(enemy))
                _slowedEnemies.Add(enemy);
        }

        public override void OnEnemyExitedRange(EnemyRuntime enemy)
        {
            if (enemy == null) return;
            enemy.Status.RemoveSlow(this);
            _slowedEnemies.Remove(enemy);
        }

        private void RefreshSlows()
        {
            for (int i = _slowedEnemies.Count - 1; i >= 0; i--)
            {
                var e = _slowedEnemies[i];
                if (e == null) { _slowedEnemies.RemoveAt(i); continue; }
                e.Status.AddSlow(this, EffectiveSlowPercent / 100f);
            }
        }

        public override void OnBeforeDestroy()
        {
            foreach (var e in _slowedEnemies)
                if (e != null) e.Status.RemoveSlow(this);
            _slowedEnemies.Clear();
        }

        public override string GetTooltipText()
        {
            return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
                   $"Type: Slowdown\n" +
                   $"Slow: -{EffectiveSlowPercent}%\n" +
                   $"Range: {Range:F1}";
        }
        
        public override string GetUpgradePreviewText()
        {
            float nextSlow  = Mathf.Min(Config.Buff + _levelBuffBonus + slow_bonus_per_level);
            float nextRange = Range + level_range_bonus + range_bonus_per_level;

            return $"Slow:\n -{EffectiveSlowPercent}% → <color={color}>-{nextSlow}%\n</color={color}>" +
                   $"Range:\n {Range:F1} → <color={color}>{nextRange:F1}</color={color}>";
        }
    }
}