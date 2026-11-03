using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowersSystem
{
  [RequireComponent(typeof(Button))]
  public class UpgradeHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    [SerializeField] private TowerTooltip tooltip;

    private Button _button;

    private void Awake()
    {
      _button = GetComponent<Button>();
      if (tooltip == null) tooltip = GetComponentInParent<TowerTooltip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (tooltip == null) return;
      if (_button != null && !_button.interactable) return;
      tooltip.ShowUpgradePreview();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (tooltip == null) return;
      tooltip.HideUpgradePreview();
    }
  }
}