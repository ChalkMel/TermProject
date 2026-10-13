using System.Collections.Generic;
using Core.Effects;

namespace TowersSystem.Stats
{ 
  public sealed class TowerStatusController : UnityEngine.MonoBehaviour
  {
    private readonly Dictionary<TowerStat, ModifierSet> _multipliers = new()
    {
      { TowerStat.OutgoingDamage, new ModifierSet(ModifierSet.Combine.Product) },
      { TowerStat.AttackSpeed,    new ModifierSet(ModifierSet.Combine.Product) }
    };

    public float DamageMultiplier      => Resolve(TowerStat.OutgoingDamage);
    public float AttackSpeedMultiplier => Resolve(TowerStat.AttackSpeed);

    private float Resolve(TowerStat stat) =>
      _multipliers.TryGetValue(stat, out var set) ? set.Resolve() : 1f;

    public void AddMultiplier(object source, TowerStat stat, float multiplier)
    {
      if (_multipliers.TryGetValue(stat, out var set)) set.Set(source, multiplier);
    }

    public void RemoveMultiplier(object source, TowerStat stat)
    {
      if (_multipliers.TryGetValue(stat, out var set)) set.Remove(source);
    }
  }
}