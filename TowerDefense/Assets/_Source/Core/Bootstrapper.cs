using AcornSystem;
using BaseSystem;
using EnemySystem;
using HeatSystem;
using LevelSystem;
using Resources;
using TowersSystem;
using TowersSystem.BuildSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Core
{
  public class Bootstrapper : MonoBehaviour
  {
    [SerializeField] private TowerPlacementController placementController;
    [SerializeField] private TowerTooltip tooltip;
    [SerializeField] private CreditsView creditsView;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private AcornSpawner acornSpawner;

    [Header("Level / Difficulty")]
    [SerializeField] private Base baseRef;
    [SerializeField] private HeatSystem.HeatSystem heatSystem;
    [Tooltip("Используется если сцена запущена напрямую (без GameSession).")]
    [SerializeField] private DifficultyConfig fallbackDifficulty;

    private Credits _credits;
    [SerializeField] private Tilemap towerTilemap;

    private PlacedTowerRegistry _registry;
    private TowerFactory _factory;
    private PlacementPreview _preview;
    private TowerInteractionController _interaction;
    private RangeIndicator _placementRange;

    private void Awake()
    {
      LevelConfig level = GameSession.CurrentLevel;
      DifficultyConfig difficulty = GameSession.CurrentDifficulty ?? fallbackDifficulty;

      SetupCredits(level, difficulty);
      SetupEnemySystem(difficulty, level);
      SetupTowerSystem();
      SetupBase(level, difficulty);
      SetupHeat(difficulty);
      timeManager.Init(_credits);
      acornSpawner.Init(_credits, difficulty);
    }

    private void SetupCredits(LevelConfig level, DifficultyConfig difficulty)
    {
      int starting = level != null ? level.StartingCredits : 120;
      float mult = difficulty?.StartingCreditsMultiplier ?? 1f;
      _credits = new Credits(Mathf.RoundToInt(starting * mult));
      creditsView.Init(_credits);
    }

    private void SetupEnemySystem(DifficultyConfig difficulty, LevelConfig level)
    {
      enemySpawner.Init(_credits, difficulty, level?.WaveGroups);
    }

    private void SetupBase(LevelConfig level, DifficultyConfig difficulty)
    {
      if (baseRef == null) return;

      float health = (level != null && level.BaseHealth > 0f) ? level.BaseHealth : baseRef.MaxHealth;
      float mult = difficulty?.BaseHealthMultiplier ?? 1f;
      baseRef.ConfigureMaxHealth(health * mult);
    }

    private void SetupHeat(DifficultyConfig difficulty)
    {
      if (heatSystem == null) return;
      heatSystem.ApplyDifficulty(difficulty);
    }

    private void SetupTowerSystem()
    {
      _placementRange = new GameObject("PlacedRangeIndicator").AddComponent<RangeIndicator>();
      _placementRange.SetColor(new Color(1f, 1f, 0f, 0.6f));

      _registry = new PlacedTowerRegistry();
      _factory  = new TowerFactory(towerTilemap);
      _preview  = placementController.gameObject.AddComponent<PlacementPreview>();
      _interaction = new TowerInteractionController(_credits, tooltip, _placementRange);

      placementController.Initialize(_registry, _factory, _preview, _interaction, _credits);
    }

    private void OnDestroy() => _interaction?.Dispose();
  }
}