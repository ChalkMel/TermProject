using _Source.Resources;
using _Source.TowersSystem.Menu;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace _Source.TowersSystem
{
    public class TowerBuilder : MonoBehaviour
    {
        [SerializeField] private TowerMenu menu;
        [SerializeField] private TowerTooltip tooltip;
        [SerializeField] private Credits credits;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Camera cam;
        [SerializeField] private SpriteRenderer highlighter;
        [SerializeField] private Transform towersParent;
        
        private Dictionary<Vector3Int, TowerRuntime> _placedTowers = new Dictionary<Vector3Int, TowerRuntime>();

        private Vector3Int _pendingCell;
        private bool _hasPendingCell;

        private void Awake()
        {
            if (menu != null)
            {
                menu.TowerChosen += OnTowerChosen;
                menu.Hide();
            }

            if (highlighter != null)
                highlighter.gameObject.SetActive(false);

            if (towersParent == null)
            {
                var go = new GameObject("Towers");
                towersParent = go.transform;
            }
        }

        private void OnDestroy()
        {
            if (menu != null)
                menu.TowerChosen -= OnTowerChosen;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (_hasPendingCell)
                {
                    CancelPlacement();
                }
                else if (tooltip != null && tooltip.IsVisible)
                {
                    tooltip.Hide();
                }
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;
                Vector3Int cellPos = tilemap.WorldToCell(worldPos);

                HandleCellClick(cellPos);
            }
        }

        private void HandleCellClick(Vector3Int cellPos)
        {
            if (_placedTowers.TryGetValue(cellPos, out TowerRuntime existingTower))
            {
                if (tooltip != null)
                {
                    tooltip.Show(existingTower.Config);
                    PositionTooltip(cellPos);
                }
                return;
            }
            
            if (tilemap.GetTile(cellPos) != null) return;

            _pendingCell = cellPos;
            _hasPendingCell = true;

            ShowHighlighter(cellPos);

            if (menu != null)
            {
                menu.Show();
            }
        }

        private void OnTowerChosen(TowerConfig tower)
        {
            if (!_hasPendingCell) return;

            if (credits.TrySpendMoney(tower.Cost))
            {
                PlaceTower(tower);
            }

            CancelPlacement();
        }

        private void PlaceTower(TowerConfig config)
        {
            if (config == null || config.Tile == null) return;
            
            tilemap.SetTile(_pendingCell, config.Tile);
            
            GameObject towerObj = new GameObject($"Tower_{config.Name}");
            towerObj.transform.SetParent(towersParent);
            towerObj.transform.position = tilemap.GetCellCenterWorld(_pendingCell);

            TowerRuntime runtime = towerObj.AddComponent<TowerRuntime>();
            runtime.Initialize(config, _pendingCell);
            SetupTowerRangeDetection(runtime);
            
            _placedTowers[_pendingCell] = runtime;

            Debug.Log($"Tower placed at {_pendingCell}");
        }
        
        private void SetupTowerRangeDetection(TowerRuntime tower)
        {
            CircleCollider2D rangeTrigger = tower.gameObject.AddComponent<CircleCollider2D>();
            rangeTrigger.radius = tower.Range;
            rangeTrigger.isTrigger = true;
            
            TowerRangeDetector detector = tower.gameObject.AddComponent<TowerRangeDetector>();
            detector.Initialize(tower);
        }

        private void CancelPlacement()
        {
            _hasPendingCell = false;
            HideHighlighter();

            if (menu != null)
                menu.Hide();
        }

        private void ShowHighlighter(Vector3Int cell)
        {
            if (highlighter == null) return;
            highlighter.gameObject.SetActive(true);
            highlighter.transform.position = tilemap.GetCellCenterWorld(cell);
        }

        private void HideHighlighter()
        {
            if (highlighter != null)
                highlighter.gameObject.SetActive(false);
        }

        private void PositionTooltip(Vector3Int cell)
        {
            if (tooltip == null) return;
            Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
            tooltip.transform.position = screenPos;
        }

        public TowerRuntime GetTowerAt(Vector3Int cell)
        {
            _placedTowers.TryGetValue(cell, out TowerRuntime runtime);
            return runtime;
        }
    }
}