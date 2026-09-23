using System.Collections.Generic;
using _Source.EnemySystem;
using UnityEngine;

namespace _Source.TowersSystem.Runtimes
{
    public class SlowdownTowerRuntime : TowerRuntimeBase
    {
        private readonly int _slowBonusPerLevel = 10;
        private readonly int _maxSlowPercent = 90;

        private readonly List<EnemyRuntime> _slowedEnemies = new();
        private int _levelBuffBonus;
        
        public float EffectiveSlowPercent =>
            Mathf.Min(Config.Buff + _levelBuffBonus, _maxSlowPercent);

        protected override Color GizmoColor => new Color(0.4f, 0.7f, 1f);

        public override void UpgradeTower()
        {
            float before = EffectiveSlowPercent;
            base.UpgradeTower();
            _levelBuffBonus += _slowBonusPerLevel;

            float after = EffectiveSlowPercent;
            float delta = after - before;
            if (delta <= 0) return;
            
            foreach (var e in _slowedEnemies)
            {
                if (e == null) continue;
                e.RemoveSlow(before / 100f);
                e.AddSlow(after / 100f);
            }
        }

        public override void OnEnemyEnteredRange(EnemyRuntime enemy)
        {
            if (enemy == null) return;

            enemy.AddSlow(EffectiveSlowPercent / 100f);
            if (!_slowedEnemies.Contains(enemy))
                _slowedEnemies.Add(enemy);
        }

        public override void OnEnemyExitedRange(EnemyRuntime enemy)
        {
            if (enemy == null) return;

            enemy.RemoveSlow(EffectiveSlowPercent / 100f);
            _slowedEnemies.Remove(enemy);
        }

        public override void OnBeforeDestroy()
        {
            float value = EffectiveSlowPercent / 100f;
            foreach (var e in _slowedEnemies)
                if (e != null) e.RemoveSlow(value);
            _slowedEnemies.Clear();
        }

        public override string GetTooltipText()
        {
            return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
                   $"Type: Slowdown\n" +
                   $"Slow: -{EffectiveSlowPercent}%\n" +
                   $"Range: {Range:F1}";
        }
    }
}