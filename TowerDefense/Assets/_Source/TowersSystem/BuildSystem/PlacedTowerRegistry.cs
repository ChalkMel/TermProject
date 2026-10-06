using System.Collections.Generic;
using TowersSystem.Runtimes;
using UnityEngine;

namespace TowersSystem.BuildSystem
{
  public class PlacedTowerRegistry
  {
    private readonly Dictionary<Vector3Int, TowerRuntimeBase> _towers = new();

    public bool TryGet(Vector3Int cell, out TowerRuntimeBase tower)
      => _towers.TryGetValue(cell, out tower);

    public void Add(Vector3Int cell, TowerRuntimeBase tower)
      => _towers[cell] = tower;

    public bool Remove(Vector3Int cell)
      => _towers.Remove(cell);

    public bool Contains(Vector3Int cell)
      => _towers.ContainsKey(cell);
  }
}