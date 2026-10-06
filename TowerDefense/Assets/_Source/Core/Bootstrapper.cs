using Resources;
using TowersSystem;
using TowersSystem.BuildSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core
{
  public class Bootstrapper : MonoBehaviour
  {
    [SerializeField] private TowerPlacementController placementController;
    [SerializeField] private TowerTooltip tooltip;
    [SerializeField] private Credits credits;
    [SerializeField] private Tilemap towerTilemap;

    private void Awake()
    {
      var placedRange = new GameObject("PlacedRangeIndicator")
        .AddComponent<RangeIndicator>();
      placedRange.SetColor(new Color(1f, 1f, 0f, 0.6f));

      var registry = new PlacedTowerRegistry();
      var factory = new TowerFactory(towerTilemap);
      var preview = placementController.gameObject.AddComponent<PlacementPreview>();
      var interaction = new TowerInteractionController(credits, tooltip, placedRange);

      placementController.Initialize(registry, factory, preview, interaction);
    }
  }
}