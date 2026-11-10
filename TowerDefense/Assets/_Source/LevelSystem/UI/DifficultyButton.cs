using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelSystem.UI
{
  [RequireComponent(typeof(Button))]
  public class DifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [Header("UI refs")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Colors")]
    [SerializeField] private Color normalColor   = new Color(0.15f, 0.18f, 0.22f, 1f);
    [SerializeField] private Color hoverColor    = new Color(0.22f, 0.27f, 0.32f, 1f);
    [SerializeField] private Color selectedColor = new Color(0.30f, 0.55f, 0.35f, 1f);

    [Header("Optional per-difficulty accent")]
    [SerializeField] private Color accentColor = new Color(0.9f, 0.85f, 0.5f, 1f);

    private Button _button;
    private bool _selected;

    public DifficultyConfig Config { get; private set; }
    public System.Action<DifficultyButton> OnClick;

    private void Awake()
    {
      _button = GetComponent<Button>();
      _button.onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public void Bind(DifficultyConfig config, string descriptionText = null)
    {
      Config = config;

      if (label != null)
        label.text = !string.IsNullOrEmpty(config.DisplayName)
          ? config.DisplayName
          : config.name;

      if (description != null)
      {
        description.text = descriptionText ?? BuildAutoDescription(config);
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