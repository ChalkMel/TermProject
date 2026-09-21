using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenuButton : MonoBehaviour
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
        text.text = $"{_tower.Name}\n" +
                    $"Cost:{_tower.Cost}\n" +
                    $"Damage:{_tower.Damage}\n" +
                    $"Cooldown:{_tower.Cooldown}";
        var image = GetComponent<Image>();
        image.sprite = _tower.tile.sprite;
    }

    private void ChooseTower()
    {
      Menu.CurrentTower = _tower;
      Debug.Log(Menu.CurrentTower);
    }
}
