using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenu : MonoBehaviour
{
    public TowerConfig CurrentTower;
    [SerializeField] private List<TowerConfig> towers;
    [SerializeField] private GameObject buttonPrefab;

    private void Awake()
    {
        DrawButtons(); 
        CurrentTower = towers[0];
        Debug.Log(CurrentTower);
    }

    private void DrawButtons()
    {
        for (int i = 0; i < towers.Count; i++)
        {
            var button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, transform);
            var comp = button.GetComponentInChildren<TowerMenuButton>();
            comp.DrawText(towers[i]);
            comp.Menu = this;
        }
    }
}
