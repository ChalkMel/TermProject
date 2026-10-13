using System.Collections.Generic;
using UnityEngine;
using EnemySystem;

namespace TowersSystem.Runtimes
{
    public class SlowdownTowerRuntime : TowerRuntimeBase
    {
        [SerializeField] private int slowBonusPerLevel = 10;
        [SerializeField] private int maxSlowPercent = 90;

        private readonly List<EnemyRuntime> _slowedEnemies = new();
        private int _levelBuffBonus;

        public float EffectiveSlowPercent =>
            Mathf.Min(Config.Buff + _levelBuffBonus, maxSlowPercent);

        protected override Color GizmoColor => new Color(0.4f, 0.7f, 1f);

        public override void UpgradeTower()
        {
            base.UpgradeTower();
            _levelBuffBonus += slowBonusPerLevel;
            RefreshSlows();
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
    }
}