using EnemySystem.Status;
using UnityEngine;

namespace EnemySystem.Auras
{
  public class EnemyStatAura : EnemyAuraEmitter
  {
    [SerializeField] private EnemyStat targetStat = EnemyStat.OutgoingDamage;
    [Tooltip("Множитель: 1.1 = +10%, 0.9 = -10%.")]
    [SerializeField] private float multiplier = 1.1f;

    protected override void OnEnemyEntered(EnemyRuntime enemy)
    {
      enemy.Status.AddMultiplier(this, targetStat, multiplier);
    }

    protected override void OnEnemyExited(EnemyRuntime enemy)
    {
      enemy.Status.RemoveMultiplier(this, targetStat);
    }
  }
}