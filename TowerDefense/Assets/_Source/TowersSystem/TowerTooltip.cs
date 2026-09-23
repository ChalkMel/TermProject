using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Source.TowersSystem
{
  public class TowerTooltip : MonoBehaviour
  {
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI sellPriceText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Vector3 offset;

    private TowerRuntimeBase _currentTower;
    private TowerBuilder _builder;

    public System.Action OnHidden;

    private void Awake()
    {
      upgradeButton.onClick.AddListener(OnUpgradeClicked);
      sellButton.onClick.AddListener(OnSellClicked);
      Hide();
    }

    public void SetBuilder(TowerBuilder builder) => _builder = builder;

    public void Show(TowerRuntimeBase tower)
    {
      _currentTower = tower;
      gameObject.SetActive(true);

      infoText.text = tower.GetTooltipText();
      upgradeCostText.text = $"Upgrade: {tower.GetUpgradeCost()}$";
      sellPriceText.text = $"Sell: {tower.GetSellPrice()}$";
    }

    public void Hide()
    {
      bool wasVisible = gameObject.activeSelf;
      _currentTower = null;
      gameObject.SetActive(false);

      if (wasVisible) OnHidden?.Invoke();
    }

    private void OnUpgradeClicked()
    {
      if (_currentTower != null && _builder != null)
        _builder.UpgradeTower(_currentTower);
    }

    private void OnSellClicked()
    {
      if (_currentTower != null && _builder != null)
      {
        _builder.SellTower(_currentTower);
        Hide();
      }
    }
  }
}