using System.Collections.Generic;
using _Source.TowersSystem.Runtimes;
using UnityEngine;

namespace _Source.TowersSystem
{
    public class BufferTowerRuntime : TowerRuntimeBase
    {
        [Header("Upgrade Bonus")]
        [SerializeField] private int buffBonusPerLevel = 5; // +5% за уровень

        private readonly List<AttackerTowerRuntime> _buffedAttackers = new();
        private int _levelBuffBonus;
        
        public int EffectiveBuff => (int) (Config.Buff + _levelBuffBonus);

        protected override Color GizmoColor => Color.cyan;

        public override void UpgradeTower()
        {
            base.UpgradeTower();
            _levelBuffBonus += buffBonusPerLevel;

            // Сообщаем всем атакующим, что наш % изменился
            foreach (var a in _buffedAttackers)
                if (a != null) a.OnBufferValueChanged();
        }

        public override void OnTowerEnteredRange(TowerRuntimeBase other)
        {
            if (other is AttackerTowerRuntime attacker)
            {
                attacker.RegisterBuffer(this);
                if (!_buffedAttackers.Contains(attacker))
                    _buffedAttackers.Add(attacker);
            }
        }

        public override void OnTowerExitedRange(TowerRuntimeBase other)
        {
            if (other is AttackerTowerRuntime attacker)
            {
                attacker.UnregisterBuffer(this);
                _buffedAttackers.Remove(attacker);
            }
        }

        public override void OnBeforeDestroy()
        {
            foreach (var a in _buffedAttackers)
                if (a != null) a.UnregisterBuffer(this);
            _buffedAttackers.Clear();
        }

        public override string GetTooltipText()
        {
            return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
                   $"Type: Buffer\n" +
                   $"Damage Buff: +{EffectiveBuff}%\n" +
                   $"Range: {Range:F1}";
        }
    }
}