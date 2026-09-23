using System.Collections.Generic;
using _Source.EnemySystem;
using UnityEngine;

namespace _Source.TowersSystem.Runtimes
{
  public class SlowdownTowerRuntime : TowerRuntimeBase
  {
    private readonly List<EnemyRuntime> _slowedEnemies = new();

    protected override Color GizmoColor => new Color(0.4f, 0.7f, 1f);

    public override void OnEnemyEnteredRange(EnemyRuntime enemy)
    {
      if (enemy == null) return;

      enemy.AddSlow(tower.Buff / 100f);
      if (!_slowedEnemies.Contains(enemy))
        _slowedEnemies.Add(enemy);
    }

    public override void OnEnemyExitedRange(EnemyRuntime enemy)
    {
      if (enemy == null) return;

      enemy.RemoveSlow(tower.Buff / 100f);
      _slowedEnemies.Remove(enemy);
    }

    public override void OnBeforeDestroy()
    {
      foreach (var e in _slowedEnemies)
        if (e != null) e.RemoveSlow(tower.Buff / 100f);
      _slowedEnemies.Clear();
    }

    public override string GetTooltipText()
    {
      return $"<b>{Config.Name}</b> (Lv. {Level})\n" +
             $"Type: Slowdown\n" +
             $"Slow: -{Config.Buff}%\n" +
             $"Range: {Range:F1}";
    }
  }
}