using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Source.TowersSystem.Menu
{
  public class TowerMenuButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
  {
    [SerializeField] private TextMeshProUGUI text;
    public TowerMenu Menu;

    private TowerConfig _tower;

    private void Awake()
    {
      GetComponent<Button>().onClick.AddListener(ChooseTower);
    }

    public void DrawText(TowerConfig tower)
    {
      _tower = tower;
      text.text = $"{_tower.Name}";
      var image = GetComponent<Image>();
      image.sprite = _tower.Tile.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (_tower != null)
        Menu.NotifyHoverStarted(_tower, eventData.position);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
      if (_tower != null)
        Menu.NotifyHoverMoved(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (_tower == null) return;
      if (eventData.pointerEnter != null)
      {
        var next = eventData.pointerEnter.GetComponentInParent<TowerMenuButton>();
        if (next != null) return;
      }

      Menu.NotifyHoverEnded(_tower);
    }

    private void ChooseTower() => Menu.SelectTower(_tower);
  }
}