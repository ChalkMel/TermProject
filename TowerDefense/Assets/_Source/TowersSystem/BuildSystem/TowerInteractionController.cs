using Resources;
using TowersSystem.Runtimes;
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

    public void Show(TowerRuntimeBase tower, Vector3 screenPos)
    {
      _tooltip.Show(tower);
      _tooltip.transform.position = screenPos;
      _range.Show(tower.transform.position, tower.Range);
    }

    public void Hide() => _tooltip.Hide();

    public bool TryUpgrade(TowerRuntimeBase tower)
    {
      if (tower == null) return false;
      if (!_credits.TrySpendMoney(tower.GetUpgradeCost())) return false;

      tower.UpgradeTower();
      _tooltip.Show(tower);
      _range.Show(tower.transform.position, tower.Range);
      return true;
    }

    public void Sell(TowerRuntimeBase tower)
    {
      if (tower == null) return;
      _credits.AddMoney(tower.GetSellPrice());
      _tooltip.Hide();
    }
  }
}