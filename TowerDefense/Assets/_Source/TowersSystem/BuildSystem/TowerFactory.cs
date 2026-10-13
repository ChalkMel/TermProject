using TowersSystem.Config;
using TowersSystem.Runtimes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TowersSystem.BuildSystem
{
  public class TowerFactory
  {
    private readonly Tilemap _tilemap;

    public TowerFactory(Tilemap tilemap) => _tilemap = tilemap;

    public TowerRuntimeBase Create(TowerConfig config, Vector3Int cell)
    {
      _tilemap.SetTile(cell, config.Tile);

      var go = new GameObject($"Tower_{config.Name}");
      go.transform.position = _tilemap.GetCellCenterWorld(cell);

      var rb = go.AddComponent<Rigidbody2D>();
      rb.bodyType = RigidbodyType2D.Kinematic;

      var runtime = AddTowerComponent(go, config.Type);
      runtime.Initialize(config, cell);

      return runtime;
    }

    private static TowerRuntimeBase AddTowerComponent(GameObject go, TowerType type) =>
      type switch
      {
        TowerType.Attacker => go.AddComponent<AttackerTowerRuntime>(),
        TowerType.Buffer   => go.AddComponent<BufferTowerRuntime>(),
        TowerType.Slowdown => go.AddComponent<SlowdownTowerRuntime>(),
        _                  => go.AddComponent<AttackerTowerRuntime>()
      };

    private static void SetupRangeDetection(TowerRuntimeBase tower)
    {
      var trigger = tower.gameObject.AddComponent<CircleCollider2D>();
      trigger.radius = tower.Range;
      trigger.isTrigger = true;
      tower.gameObject.AddComponent<TowerRangeDetector>().Initialize(tower);
    }

    public void Remove(TowerRuntimeBase tower)
    {
      if (tower == null) return;

      _tilemap.SetTile(tower.CellPosition, null);
      tower.OnBeforeDestroy();
      Object.Destroy(tower.gameObject);
    }
  }
}