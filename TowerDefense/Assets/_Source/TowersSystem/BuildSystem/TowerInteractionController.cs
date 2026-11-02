using Resources;
using TowersSystem.Runtimes;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace TowersSystem.BuildSystem
{
  public class TowerInteractionController : System.IDisposable
  {
    private readonly Credits _credits;
    private readonly TowerTooltip _tooltip;
    private readonly RangeIndicator _range;
    private bool _disposed;

    public TowerInteractionController(Credits credits, TowerTooltip tooltip, RangeIndicator range)
    {
      _credits = credits;
      _tooltip = tooltip;
      _range   = range;
      _tooltip.OnHidden += HandleTooltipHidden;
    }

    private void HandleTooltipHidden() => _range.Hide();

    public void Dispose()
    {
      if (_disposed) return;
      _disposed = true;
      _tooltip.OnHidden -= HandleTooltipHidden;
    }

    public void Show(TowerRuntimeBase tower, Vector3 screenPosOfTower)
    {
      _tooltip.Show(tower, screenPosOfTower);
      _range.Show(tower.transform.position, tower.Range);
    }

    public bool TryUpgrade(TowerRuntimeBase tower)
    {
      if (tower == null) return false;
      if (!_credits.TrySpendMoney(tower.GetUpgradeCost())) return false;

      tower.UpgradeTower();
      var screen = Camera.main.WorldToScreenPoint(tower.transform.position);
      _tooltip.Show(tower, screen);
      _range.Show(tower.transform.position, tower.Range);
      return true;
    }

    public void Hide() => _tooltip.Hide();

    public void Sell(TowerRuntimeBase tower)
    {
      if (tower == null) return;
      _credits.AddMoney(tower.GetSellPrice());
      _tooltip.Hide();
    }
  }
}