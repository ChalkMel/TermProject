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

    public TowerConfig SelectedTower { get; private set; }

    private readonly List<TowerMenuButton> _buttons = new();

    private void Awake()
    {
      DrawButtons();
      if (menu != null) menu.SetActive(true);
    }

    public void Show()
    {
      if (menu != null) menu.SetActive(true);
      if (menuTooltip != null) menuTooltip.Hide();
    }

    public void Hide()
    {
      if (menu != null) menu.SetActive(false);
      if (menuTooltip != null) menuTooltip.Hide();
    }

    private void DrawButtons()
    {
      _buttons.Clear();
      foreach (TowerConfig t in towers)
      {
        var button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, transform);
        var comp = button.GetComponentInChildren<TowerMenuButton>();
        comp.Draw(t);
        comp.Menu = this;
        _buttons.Add(comp);
      }
    }
    
    public void SelectTower(TowerConfig tower)
    {
      var newSelection = (SelectedTower == tower) ? null : tower;
      SetSelection(newSelection);
      TowerChosen?.Invoke(SelectedTower);
    }

    public void ClearSelection()
    {
      if (SelectedTower == null) return;
      SetSelection(null);
      TowerChosen?.Invoke(null);
    }

    private void SetSelection(TowerConfig tower)
    {
      SelectedTower = tower;
      foreach (var b in _buttons)
        b.SetSelected(tower != null && b.Tower == tower);
    }

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