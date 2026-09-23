using UnityEngine;
using _Source.EnemySystem;

namespace _Source.TowersSystem
{
  public abstract class TowerRuntimeBase : MonoBehaviour
  {
    [SerializeField] protected TowerConfig tower;

    public TowerConfig Config => tower;
    public Vector3Int CellPosition { get; private set; }
    public int Level { get; private set; } = 1;

    protected float _levelRangeBonus;

    public virtual float Range => tower.Range + _levelRangeBonus;

    public void Initialize(TowerConfig config, Vector3Int cellPosition)
    {
      tower = config;
      CellPosition = cellPosition;
      name = $"Tower_{config.Name}";
      OnInitialized();
    }

    protected virtual void OnInitialized() { }

    public virtual int GetUpgradeCost() => Config.Cost * Level;
    public virtual int GetSellPrice() => Mathf.Max(1, (Config.Cost * Level) / 2);

    public virtual void UpgradeTower()
    {
      Level++;
      _levelRangeBonus += 0.5f;
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
      if (tower == null) return;
      Gizmos.color = GizmoColor;
      Gizmos.DrawWireSphere(transform.position, Range);
    }
  }
}