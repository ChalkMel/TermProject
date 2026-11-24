using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelSystem.UI
{
  [RequireComponent(typeof(Button))]
  public class DifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Color normalColor   = new(0.15f, 0.18f, 0.22f, 1f);
    [SerializeField] private Color hoverColor    = new(0.22f, 0.27f, 0.32f, 1f);
    [SerializeField] private Color selectedColor = new(0.30f, 0.55f, 0.35f, 1f);
    [SerializeField] private Color accentColor   = new(0.9f, 0.85f, 0.5f, 1f);

    private Button _button;
    private bool _selected;
    private DifficultyConfig _config;

    public DifficultyConfig Config => _config;
    public event Action<DifficultyConfig> Clicked;

    private void Awake()
    {
      _button = GetComponent<Button>();
      _button.onClick.AddListener(() => Clicked?.Invoke(_config));
    }

    public void Bind(DifficultyViewModel vm)
    {
      _config = vm.Config;

      if (label != null) label.text = vm.Label;

      if (description != null)
      {
        description.text = vm.Description;
        if (accentColor.a > 0f) description.color = accentColor;
      }

      SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
      _selected = selected;
      if (background != null)
        background.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (!_selected && background != null) background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (!_selected && background != null) background.color = normalColor;
    }

    private static string BuildAutoDescription(DifficultyConfig d)
    {
      var sb = new System.Text.StringBuilder();

      void Add(string label, float v, bool percentIncrease)
      {
        if (Mathf.Approximately(v, 1f)) return;
        float pct = percentIncrease ? (v - 1f) * 100f : -(v - 1f) * 100f;
        string sign = pct >= 0f ? "+" : "";
        sb.AppendLine($"{label}: {sign}{pct:F0}%");
      }

      Add("Enemy HP",   d.EnemyHealthMultiplier, true);
      Add("Enemy DMG",  d.EnemyDamageMultiplier, true);
      Add("Enemy SPD",  d.EnemySpeedMultiplier,  true);
      Add("Rewards",    d.EnemyRewardMultiplier, true);
      Add("Base HP",    d.BaseHealthMultiplier,  true);

      return sb.ToString().TrimEnd();
    }
  }
}