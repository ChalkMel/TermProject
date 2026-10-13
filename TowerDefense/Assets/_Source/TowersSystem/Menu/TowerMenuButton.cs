using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TowersSystem.Config;

namespace TowersSystem.Menu
{
  public class TowerMenuButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
  {
    [SerializeField] private Image image;
    [SerializeField] private Color selectedColor = new Color(0.4f, 0.4f, 0.4f);
    public TowerMenu Menu;

    private Button _button;
    private Color _normalColor;

    public TowerConfig Tower { get; private set; }

    private void Awake()
    {
      _button = GetComponent<Button>();
      _button.onClick.AddListener(ChooseTower);
      _normalColor = image.color;
    }

    public void Draw(TowerConfig tower)
    {
      Tower = tower;
      image.sprite = Tower.Tile.sprite;
    }

    public void SetSelected(bool selected)
    {
      image.color = selected ? selectedColor : _normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (Tower != null)
        Menu.NotifyHoverStarted(Tower, (RectTransform)transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (Tower == null) return;
      
      if (eventData.pointerEnter != null)
      {
        var next = eventData.pointerEnter.GetComponentInParent<TowerMenuButton>();
        if (next != null && next != this) return;
      }

      Menu.NotifyHoverEnded(Tower);
    }

    private void ChooseTower() => Menu.SelectTower(Tower);
  }
}