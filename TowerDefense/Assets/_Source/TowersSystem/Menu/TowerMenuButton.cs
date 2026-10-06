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
      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (_tower != null)
        Menu.NotifyHoverStarted(_tower, (RectTransform)transform);
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