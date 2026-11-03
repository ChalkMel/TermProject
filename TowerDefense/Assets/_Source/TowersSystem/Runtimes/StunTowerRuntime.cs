using UnityEngine;

namespace TowersSystem.Runtimes
{
  public class StunTowerRuntime : AttackerTowerRuntime
  {
    [SerializeField] private float stunBonusPerLevel = 0.15f;
    private int maxLevel = 3;
    private float _levelStunBonus;

    protected override float StunOnHit => Config.StunDuration + _levelStunBonus;

    protected override Color GizmoColor => new Color(0.8f, 0.4f, 1f);

    public override bool UpgradeTower()
    {
      if (!base.UpgradeTower()) return false;
      _levelStunBonus += stunBonusPerLevel;
      return true;
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

      return $"Stun:\n {StunOnHit:F2}s → <color={color}>{nextStun:F2}s</color={color}>\n" +
             $"Range:\n {Range:F1} → <color={color}>{nextRange:F1}</color={color}>";
    }
  }
}