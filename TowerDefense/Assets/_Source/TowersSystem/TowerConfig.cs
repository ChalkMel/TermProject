using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TowerConfig : ScriptableObject {
    public string Name;
    public int Cost;
    public float Range;
    public float Cooldown;
    public int Damage;
    public Tile tile;
}
