using UnityEngine;
using EnemySystem;
using TowersSystem.Config;
using TowersSystem.Stats;

namespace TowersSystem.Runtimes
{
  public abstract class TowerRuntimeBase : MonoBehaviour
  {
    protected TowerConfig Tower;

    public TowerConfig Config => Tower;
    public Vector3Int CellPosition { get; private set; }
    public int Level { get; private set; } = 1;

    public TowerStatusController Status { get; private set; }

    protected float _levelRangeBonus;

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

    public virtual int GetUpgradeCost() => Config.Cost * Level / 4;
    public virtual int GetSellPrice() => Mathf.Max(1, Config.Cost * Level / 4);

    public virtual void UpgradeTower()
    {
      Level++;
      _levelRangeBonus += .5f;
    }

    public virtual void OnEnemyEnteredRange(EnemyRuntime enemy) { }
    public virtual void OnEnemyExitedRange(EnemyRuntime enemy) { }
    public virtual void OnTowerEnteredRange(TowerRuntimeBase other) { }
    public virtual void OnTowerExitedRange(TowerRuntimeBase other) { }

    public virtual void OnBeforeDestroy() { }

    public virtual string GetTooltipText() =>
      $"<b>{Config.Name}</b> (Lv. {Level})\nRange: {Range:F1}";

    protected abstract Color GizmoColor { get; }

    private void OnDrawGizmos()
    {
      if (Tower == null) return;
      Gizmos.color = GizmoColor;
      Gizmos.DrawWireSphere(transform.position, Range);
    }
  }
}