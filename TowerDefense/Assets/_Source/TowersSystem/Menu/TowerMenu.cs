using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Source.TowersSystem.Menu
{ 
    public class TowerMenu : MonoBehaviour
    {
        [SerializeField] private List<TowerConfig> towers;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject menu;
        public event Action<TowerConfig> TowerChosen;

        private void Awake()
        {
            DrawButtons();
            Hide();
        }

        public void Show()
        {
            menu.SetActive(true);
        }

        public void Hide()
        {
            menu.SetActive(false);
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

        public void SelectTower(TowerConfig tower)
        {
            TowerChosen?.Invoke(tower);
        }
    }
}
