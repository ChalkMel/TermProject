using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelSystem.UI
{
  [RequireComponent(typeof(Button))]
  public class LevelCardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [Header("UI refs")]
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Header("Colors")]
    [SerializeField] private Color normalColor   = new Color(0.15f, 0.18f, 0.22f, 1f);
    [SerializeField] private Color hoverColor    = new Color(0.22f, 0.27f, 0.32f, 1f);
    [SerializeField] private Color selectedColor = new Color(0.30f, 0.55f, 0.35f, 1f);
    [SerializeField] private Color lockedColor   = new Color(0.08f, 0.08f, 0.10f, 1f);

    [Header("Badges (optional)")]
    [SerializeField] private GameObject lockBadge;
    [SerializeField] private GameObject completedBadge;

    private Button _button;
    private bool _selected;
    private bool _locked;

    public LevelConfig Config { get; private set; }
    public bool IsLocked => _locked;

    public System.Action<LevelCardButton> OnClick;

    private void Awake()
    {
      _button = GetComponent<Button>();
      _button.onClick.AddListener(HandleClick);
    }
    
    public void Bind(LevelConfig config, bool locked, bool completed, Sprite iconSprite = null)
    {
      Config = config;
      _locked = locked;

      if (titleText != null)
        titleText.text = string.IsNullOrEmpty(config.DisplayName)
          ? $"Level {config.LevelNumber}"
          : config.DisplayName;

      if (subtitleText != null)
        subtitleText.text = locked
          ? "Locked"
          : (completed ? "Completed" : $"Level {config.LevelNumber}");

      if (icon != null)
      {
        icon.sprite = config.LevelIcon;
        icon.enabled = true;
      }

      if (lockBadge != null)      lockBadge.SetActive(locked);
      if (completedBadge != null) completedBadge.SetActive(!locked && completed);

      _button.interactable = !locked;

      if (background != null)
        background.color = locked ? lockedColor : normalColor;

      _selected = false;
    }

    public void SetSelected(bool selected)
    {
      if (_locked) return;
      _selected = selected;
      if (background != null)
        background.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (_locked) return;
      if (!_selected && background != null) background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (_locked) return;
      if (!_selected && background != null) background.color = normalColor;
    }

    private void HandleClick()
    {
      if (_locked) return;
      OnClick?.Invoke(this);
    }
  }
}