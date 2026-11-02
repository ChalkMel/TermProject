using TMPro;
using TowersSystem.BuildSystem;
using TowersSystem.Runtimes;
using UnityEngine;
using UnityEngine.UI;

namespace TowersSystem
{
  public class TowerTooltip : MonoBehaviour
  {
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI sellPriceText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;

    [Header("Positioning")]
    [Tooltip("Смещение панели относительно экранной точки башни (в пикселях UI).")]
    [SerializeField] private Vector2 screenOffset = new Vector2(-120f, 0f);
    [SerializeField] private float screenEdgePadding = 8f;

    private RectTransform _rect;
    private RectTransform _canvasRect;
    private Canvas _canvas;
    private Camera _uiCamera;

    private TowerRuntimeBase _currentTower;
    private TowerPlacementController _builder;

    public System.Action OnHidden;

    private void Awake()
    {
      _rect = transform as RectTransform;
      _canvas = GetComponentInParent<Canvas>();
      if (_canvas != null)
      {
        _canvasRect = _canvas.transform as RectTransform;
        if (_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
          _uiCamera = _canvas.worldCamera;
      }

      upgradeButton.onClick.AddListener(OnUpgradeClicked);
      sellButton.onClick.AddListener(OnSellClicked);
      Hide();
    }

    public void SetBuilder(TowerPlacementController builder) => _builder = builder;

    public void Show(TowerRuntimeBase tower, Vector2 screenPosOfTower)
    {
      _currentTower = tower;
      gameObject.SetActive(true);

      infoText.text        = tower.GetTooltipText();
      upgradeCostText.text = $"Upgrade: {tower.GetUpgradeCost()}";
      sellPriceText.text   = $"Sell: {tower.GetSellPrice()}";
      
      LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

      PositionNearScreenPoint(screenPosOfTower);
    }
    public void Show(TowerRuntimeBase tower) => Show(tower, _rect.position);

    public void Hide()
    {
      bool wasVisible = gameObject.activeSelf;
      _currentTower = null;
      gameObject.SetActive(false);
      if (wasVisible) OnHidden?.Invoke();
    }

    private void PositionNearScreenPoint(Vector2 screenPosOfTower)
    {
      if (_rect == null || _canvasRect == null) return;
      
      RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _canvasRect, screenPosOfTower, _uiCamera, out Vector2 localPoint);
      
      Vector2 desired = localPoint + screenOffset;
      
      Vector2 canvasSize  = _canvasRect.rect.size;
      Vector2 canvasPivot = _canvasRect.pivot;
      float minX = -canvasSize.x * canvasPivot.x + screenEdgePadding;
      float maxX =  canvasSize.x * (1f - canvasPivot.x) - screenEdgePadding;
      float minY = -canvasSize.y * canvasPivot.y + screenEdgePadding;
      float maxY =  canvasSize.y * (1f - canvasPivot.y) - screenEdgePadding;
      
      Vector2 panelSize = _rect.rect.size;
      Vector2 panelPivot = _rect.pivot;

      float px = Mathf.Clamp(desired.x,
                             minX + panelSize.x * panelPivot.x,
                             maxX - panelSize.x * (1f - panelPivot.x));
      float py = Mathf.Clamp(desired.y,
                             minY + panelSize.y * panelPivot.y,
                             maxY - panelSize.y * (1f - panelPivot.y));
      
      _rect.anchoredPosition = new Vector2(px, py);
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