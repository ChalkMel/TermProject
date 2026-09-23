using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Source.TowersSystem
{
  public enum TowerType
  {
    Attacker,
    Buffer,
    Slowdown
  }

  [CreateAssetMenu(fileName = "NewTowerConfig", menuName = "Towers/Tower Config")]
  public class TowerConfig : ScriptableObject
  {
    public string Name;
    public TowerType Type = TowerType.Attacker;

    [Header("Attacker")]
    public float Cooldown;
    public int Damage;

    [Header("Buffer")]
    public float Buff = 10f;

    [Header("Common")]
    public int Cost;
    public float Range;
    public Tile Tile;
    public GameObject Prefab;
  }
}