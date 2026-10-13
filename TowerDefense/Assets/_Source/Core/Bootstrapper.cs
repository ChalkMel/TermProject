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

    private PlacedTowerRegistry _registry;
    private TowerFactory _factory;
    private PlacementPreview _preview;
    private TowerInteractionController _interaction;
    private RangeIndicator _placementRange;
    
    private void Awake()
    {
      _placementRange = new GameObject("PlacedRangeIndicator")
        .AddComponent<RangeIndicator>();
      _placementRange.SetColor(new Color(1f, 1f, 0f, 0.6f));

      _registry = new PlacedTowerRegistry();
      _factory = new TowerFactory(towerTilemap);
      _preview = placementController.gameObject.AddComponent<PlacementPreview>();
      _interaction = new TowerInteractionController(credits, tooltip, _placementRange);

      placementController.Initialize(_registry, _factory, _preview, _interaction);
    }
    
    private void OnDestroy()
    {
      _interaction?.Dispose();
    }
  }
}