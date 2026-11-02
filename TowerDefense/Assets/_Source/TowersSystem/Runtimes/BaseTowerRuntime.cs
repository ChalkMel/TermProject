using UnityEngine;
using EnemySystem;
using TowersSystem.Config;
using TowersSystem.Stats;

namespace TowersSystem.Runtimes
{
  public abstract class TowerRuntimeBase : MonoBehaviour
  {
    protected TowerConfig Tower;
    public System.Action<float> OnRangeChanged;

    public TowerConfig Config => Tower;
    public Vector3Int CellPosition { get; private set; }
    public int Level { get; private set; } = 1;

    public TowerStatusController Status { get; private set; }

    protected int maxLevel = 5;
    public bool IsMaxLevel => Level >= maxLevel;
    
    protected float _levelRangeBonus;
    protected const float level_range_bonus = .2f;
    public bool CanBeBought;

    public virtual float Range => Tower.Range + _levelRangeBonus;

    protected virtual void Awake()
    {
      Status = GetComponent<TowerStatusController>();
      if (Status == null) Status = gameObject.AddComponent<TowerStatusController>();
    }

    public void Initialize(TowerConfig config, Vector3Int cellPosition)
    {
      Tower = config;
      CellPosition = cellPosition;
      name = $"Tower_{config.Name}";
      OnInitialized();
    }

    protected virtual void OnInitialized() { }

    public virtual int GetUpgradeCost() => Mathf.Max(Config.Cost / 2 * Level);
    public virtual int GetSellPrice() => Mathf.Max(1, Config.Cost * Level / 4);

    public virtual bool UpgradeTower()
    {
      if (IsMaxLevel) return false;
      Level++;
      _levelRangeBonus += level_range_bonus;
      OnRangeChanged?.Invoke(Range);
      return true;
    }

    public virtual void OnEnemyEnteredRange(EnemyRuntime enemy) { }
    public virtual void OnEnemyExitedRange(EnemyRuntime enemy) { }
    public virtual void OnTowerEnteredRange(TowerRuntimeBase other) { }
    public virtual void OnTowerExitedRange(TowerRuntimeBase other) { }

    public virtual void OnBeforeDestroy() { }

    public virtual string GetTooltipText() =>
      $"<b>{Config.Name}</b> (Lv. {Level})\nRange: {Range:F1}";
    public virtual string GetUpgradePreviewText() =>
      null;

    protected abstract Color GizmoColor { get; }

    private void OnDrawGizmos()
    {
      if (Tower == null) return;
      Gizmos.color = GizmoColor;
      Gizmos.DrawWireSphere(transform.position, Range);
    }
  }
}