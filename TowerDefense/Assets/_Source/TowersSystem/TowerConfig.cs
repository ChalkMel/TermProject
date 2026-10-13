using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTowerConfig", menuName = "Towers/Tower Config")]
public class TowerConfig : ScriptableObject
{
    public string Name;
    public float Cooldown;
    public float Damage;
    public int Level;
    public int Cost;
    public float Range;
    public Tile Tile;
    public GameObject Prefab;
}
