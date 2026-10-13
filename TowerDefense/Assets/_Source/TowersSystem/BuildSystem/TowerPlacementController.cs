using Resources;
using TowersSystem.Config;
using TowersSystem.Menu;
using TowersSystem.Runtimes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace TowersSystem.BuildSystem
{
    public class TowerPlacementController : MonoBehaviour
    {
        [Header("Scene refs")]
        [SerializeField] private TowerMenu menu;
        [SerializeField] private Camera cam;
        [SerializeField] private Tilemap tilemap;
        private Credits _credits;

        [Header("Tooltip")]
        [SerializeField] private TowerTooltip tooltip;
        [SerializeField] private Vector3 tooltipScreenOffset = new Vector3(-100f, 0f, 0f);

        private PlacedTowerRegistry _registry;
        private TowerFactory _factory;
        private PlacementPreview _preview;
        private TowerInteractionController _interaction;

        private TowerConfig _selectedConfig;

        public void Initialize(
            PlacedTowerRegistry registry,
            TowerFactory factory,
            PlacementPreview preview,
            TowerInteractionController interaction,
            Credits credits)
        {
            _registry = registry;
            _factory = factory;
            _preview = preview;
            _interaction = interaction;
            _credits = credits;
        }

        private void Awake()
        {
            menu.TowerChosen += OnTowerChosen;

            if (tooltip != null)
            {
                tooltip.SetBuilder(this);
                tooltip.Hide();
            }
        }

        private void OnDestroy()
        {
            menu.TowerChosen -= OnTowerChosen;
        }

        private void Update()
        {
            if (_selectedConfig != null) HandlePlacementUpdate();
            else HandleTooltipUpdate();
        }

        private void HandlePlacementUpdate()
        {
            Vector3Int cell = GetCellUnderMouse();
            Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
            bool valid = CanPlace(cell);

            _preview.Show(_selectedConfig, worldPos, tilemap.cellSize, valid);

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
                if (!valid) return;

                PlaceTower(_selectedConfig, cell);
                // Выбор сохраняется — можно ставить несколько башен подряд.
            }
        }

        private void HandleTooltipUpdate()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (tooltip != null && tooltip.gameObject.activeSelf) _interaction.Hide();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
                if (tooltip != null && tooltip.gameObject.activeSelf) _interaction.Hide();

                if (_registry.TryGet(GetCellUnderMouse(), out TowerRuntimeBase existing))
                    _interaction.Show(existing, GetTooltipScreenPos(existing.CellPosition));
            }
        }

        private bool CanPlace(Vector3Int cell)
        {
            if (_registry.Contains(cell)) return false;
            if (tilemap.GetTile(cell) != null) return false;
            if (_credits.GetMoney() < _selectedConfig.Cost) return false;
            return true;
        }

        private void PlaceTower(TowerConfig config, Vector3Int cell)
        {
            if (!_credits.TrySpendMoney(config.Cost)) return;
            if (_registry.Contains(cell)) return;

            TowerRuntimeBase tower = _factory.Create(config, cell);
            _registry.TryAdd(cell, tower);
        }

        private void OnTowerChosen(TowerConfig config)
        {
            if (config == null) { CancelPlacement(); return; }

            _selectedConfig = config;
            if (tooltip != null && tooltip.gameObject.activeSelf) _interaction.Hide();
        }

        private void CancelPlacement()
        {
            if (_selectedConfig == null) return;

            _selectedConfig = null;
            _preview.Hide();
            menu.ClearSelection(); // снимет подсветку кнопки; повторно TowerChosen(null) не зацикливается
        }

        private Vector3Int GetCellUnderMouse()
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;
            return tilemap.WorldToCell(worldPos);
        }

        public void UpgradeTower(TowerRuntimeBase tower) => _interaction.TryUpgrade(tower);

        public void SellTower(TowerRuntimeBase tower)
        {
            if (tower == null) return;

            _interaction.Sell(tower);
            _registry.Remove(tower.CellPosition);
            _factory.Remove(tower);
        }

        private Vector3 GetTooltipScreenPos(Vector3Int cell)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
            return cam.WorldToScreenPoint(worldPos) + tooltipScreenOffset;
        }
    }
}