using UnityEngine;
using UnityEngine.Tilemaps;

namespace TowersSystem.Config
{
  public enum TowerType
  {
    Attacker = 0,
    Buffer   = 1,
    Slowdown = 2,
  }

  [CreateAssetMenu(fileName = "NewTowerConfig", menuName = "Towers/Tower Config")]
  public class TowerConfig : ScriptableObject
  {
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public TowerType Type { get; private set; }

    [Header("Attacker")]
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }

    [Header("Buffer / Slowdown")]
    [field: SerializeField] public float Buff { get; private set; }

    [Header("Common")]
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public Tile Tile { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
  }
}