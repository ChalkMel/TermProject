using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Source.HeatSystem
{
  public class HeatUI : MonoBehaviour
  {
    [SerializeField] private HeatSystem heatSystem;
    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI heatText;
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color overheatColor = Color.red;
    [SerializeField] private Button button;

    private void Awake()
    {
      if (heatSystem == null || fillBar == null) return;

      heatSystem.OnHeatChanged += Draw;
      heatSystem.OnOverheated += HandleOverheat;
      heatSystem.OnOverheatRecovered += HandleRecover;

      Draw(heatSystem.Heat, heatSystem.MaxHeat);
      
      button.onClick.AddListener(heatSystem.Press);
    }

    private void OnDestroy()
    {
      if (heatSystem == null) return;

      heatSystem.OnHeatChanged -= Draw;
      heatSystem.OnOverheated -= HandleOverheat;
      heatSystem.OnOverheatRecovered -= HandleRecover;
    }

    private void Draw(float heat, float max)
    {
      if (fillBar != null)
        fillBar.fillAmount = max > 0f ? heat / max : 0f;

      if (heatText != null)
        heatText.text = $"{Mathf.RoundToInt(heat)} / {Mathf.RoundToInt(max)}";
    }

    private void HandleOverheat()
    {
      if (fillBar != null) fillBar.color = overheatColor;
    }

    private void HandleRecover()
    {
      if (fillBar != null) fillBar.color = normalColor;
    }
  }
}