using System;
using System.Collections.Generic;
using UnityEngine;
using TowersSystem.Config;

namespace TowersSystem.Menu
{
  public class TowerMenu : MonoBehaviour
  {
    [SerializeField] private List<TowerConfig> towers;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject menu;
    [SerializeField] private TowerBuildButtonTooltip menuTooltip;

    public event Action<TowerConfig> TowerChosen;
    public event Action<TowerConfig> TowerHoverStarted;
    public event Action<TowerConfig> TowerHoverEnded;

    private void Awake()
    {
      DrawButtons();
      Hide();
    }

    public void Show()
    {
      menu.SetActive(true);
      if (menuTooltip != null) menuTooltip.Hide();
    }

    public void Hide()
    {
      menu.SetActive(false);
      if (menuTooltip != null) menuTooltip.Hide();
    }

    private void DrawButtons()
    {
      foreach (TowerConfig t in towers)
      {
        var button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, transform);
        var comp = button.GetComponentInChildren<TowerMenuButton>();
        comp.DrawText(t);
        comp.Menu = this;
      }
    }

    public void SelectTower(TowerConfig tower) => TowerChosen?.Invoke(tower);

    public void NotifyHoverStarted(TowerConfig tower, RectTransform buttonRect)
    {
      TowerHoverStarted?.Invoke(tower);
      if (menuTooltip != null) menuTooltip.Show(tower, buttonRect);
    }

    public void NotifyHoverEnded(TowerConfig tower)
    {
      TowerHoverEnded?.Invoke(tower);
      if (menuTooltip != null) menuTooltip.Hide();
    }
  }
}