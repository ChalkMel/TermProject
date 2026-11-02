using UnityEngine;

namespace TowersSystem.Runtimes
{
  public class StunTowerRuntime : AttackerTowerRuntime
  {
    [SerializeField] private float stunBonusPerLevel = 0.15f;

    private float _levelStunBonus;

    protected override float StunOnHit => Config.StunDuration + _levelStunBonus;

    protected override Color GizmoColor => new Color(0.8f, 0.4f, 1f);

    public override void UpgradeTower()
    {
      base.UpgradeTower();
      _levelStunBonus += stunBonusPerLevel;
    }

    public override string GetTooltipText()
    {
      float buffPercent = (Status.DamageMultiplier - 1f) * 100f;
      string buffLine = Mathf.Abs(buffPercent) > 0.01f
        ? $"\nBuff: {(buffPercent >= 0 ? "+" : "")}{buffPercent:F0}%"
        : "";

      return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
             $"Type: Stun\n" +
             $"Stun: {StunOnHit:F2}s\n" +
             $"Range: {Range:F1}\n" +
             $"Cooldown: {Config.Cooldown * Status.AttackSpeedMultiplier}s{buffLine}";
    }
    
    public override string GetUpgradePreviewText()
    {
      float nextStun  = StunOnHit + stunBonusPerLevel;
      float nextRange = Range + level_range_bonus;

      return $"Stun: {StunOnHit:F2}s → {nextStun:F2}s\n" +
             $"Range: {Range:F1} → {nextRange:F1}";
    }
  }
}