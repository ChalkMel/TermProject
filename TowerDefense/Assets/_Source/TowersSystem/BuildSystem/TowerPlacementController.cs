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
        [SerializeField] private SpriteRenderer highlighter;
        [SerializeField] private Credits credits;

        [Header("Tooltip")]
        [SerializeField] private TowerTooltip tooltip;
        [SerializeField] private Vector3 tooltipScreenOffset = new Vector3(-100f, 0f, 0f);

        private PlacedTowerRegistry _registry;
        private TowerFactory _factory;
        private PlacementPreview _preview;
        private TowerInteractionController _interaction;

        private Vector3Int _pendingCell;
        private bool _hasPendingCell;
        
        public void Initialize(
            PlacedTowerRegistry registry,
            TowerFactory factory,
            PlacementPreview preview,
            TowerInteractionController interaction)
        {
            _registry = registry;
            _factory = factory;
            _preview = preview;
            _interaction = interaction;
        }

        private void Awake()
        {
            menu.TowerChosen       += OnTowerChosen;
            menu.TowerHoverStarted += OnTowerHoverStarted;
            menu.TowerHoverEnded   += OnTowerHoverEnded;
            menu.Hide();

            if (tooltip != null)
            {
                tooltip.SetBuilder(this);
                tooltip.Hide();
            }

            if (highlighter != null)
                highlighter.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            menu.TowerChosen       -= OnTowerChosen;
            menu.TowerHoverStarted -= OnTowerHoverStarted;
            menu.TowerHoverEnded   -= OnTowerHoverEnded;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (_hasPendingCell) CancelPlacement();
                else if (tooltip != null && tooltip.gameObject.activeSelf) _interaction.Hide();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                HandleCellClick(GetCellUnderMouse());
            }
        }

        private Vector3Int GetCellUnderMouse()
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;
            return tilemap.WorldToCell(worldPos);
        }

        private void HandleCellClick(Vector3Int cell)
        {
            _preview.Hide();
            if (tooltip != null && tooltip.gameObject.activeSelf) _interaction.Hide();
            
            if (_registry.TryGet(cell, out TowerRuntimeBase existing))
            {
                _interaction.Show(existing, GetTooltipScreenPos(cell));
                return;
            }

            if (tilemap.GetTile(cell) != null) return;

            BeginPlacement(cell);
        }

        private void BeginPlacement(Vector3Int cell)
        {
            _pendingCell = cell;
            _hasPendingCell = true;

            if (highlighter != null)
            {
                highlighter.gameObject.SetActive(true);
                highlighter.transform.position = tilemap.GetCellCenterWorld(cell);
            }

            menu.Show();
        }

        private void OnTowerChosen(TowerConfig config)
        {
            if (!_hasPendingCell || config == null)
            {
                CancelPlacement();
                return;
            }

            if (credits.TrySpendMoney(config.Cost))
            {
                TowerRuntimeBase tower = _factory.Create(config, _pendingCell);
                _registry.Add(_pendingCell, tower);
            }

            CancelPlacement();
        }

        private void OnTowerHoverStarted(TowerConfig config)
        {
            if (!_hasPendingCell || config == null) return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(_pendingCell);
            _preview.Show(config, worldPos, tilemap.cellSize);

            if (highlighter != null) highlighter.gameObject.SetActive(false);
        }

        private void OnTowerHoverEnded(TowerConfig config)
        {
            _preview.Hide();

            if (_hasPendingCell && highlighter != null)
            {
                highlighter.gameObject.SetActive(true);
                highlighter.transform.position = tilemap.GetCellCenterWorld(_pendingCell);
            }
        }

        public void UpgradeTower(TowerRuntimeBase tower)
        {
            _interaction.TryUpgrade(tower);
        }

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

        private void CancelPlacement()
        {
            _hasPendingCell = false;
            menu.Hide();
            _preview.Hide();

            if (highlighter != null)
                highlighter.gameObject.SetActive(false);
        }
    }
}