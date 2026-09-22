using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Source.TowersSystem
{
  public class TowerTooltip : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button closeButton;

    private TowerConfig _currentTower;
    private bool _isVisible;

    private void Awake()
    {
      if (closeButton != null)
      {
        closeButton.onClick.AddListener(Hide);
      }
      Hide();
    }

    public void Show(TowerConfig config)
    {
      _currentTower = config;
      _isVisible = true;
      gameObject.SetActive(true);

      if (titleText != null)
        titleText.text = config.Name;

      if (statsText != null)
      {
        statsText.text = $"Damage: {config.Damage}\n" +
                         $"Range: {config.Range:F1}\n" +
                         $"Cooldown: {config.Cooldown:F1}s";
      }

      if (iconImage != null && config.Tile != null && config.Tile.sprite != null)
      {
        iconImage.sprite = config.Tile.sprite;
      }
    }

    public void Hide()
    {
      _isVisible = false;
      _currentTower = null;
      gameObject.SetActive(false);
    }

    public bool IsVisible => _isVisible;
  }
}