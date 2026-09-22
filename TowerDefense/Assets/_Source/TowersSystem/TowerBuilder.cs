using System.Collections.Generic;
using _Source.Resources;
using _Source.TowersSystem.Menu;
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
        
        private Dictionary<Vector3Int, TowerRuntime> _placedTowers = new Dictionary<Vector3Int, TowerRuntime>();

        private Vector3Int _pendingCell;
        private bool _hasPendingCell;

        private void Awake()
        {
            menu.TowerChosen += OnTowerChosen;
            menu.Hide();
            
            if (tooltip != null)
            {
                tooltip.SetBuilder(this);
                tooltip.Hide();
            }
            
            if (highlighter != null) highlighter.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            menu.TowerChosen -= OnTowerChosen;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (_hasPendingCell) CancelPlacement();
                else if (tooltip.gameObject.activeSelf) tooltip.Hide();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

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
                tooltip.Show(existingTower);
                PositionTooltip(cellPos);
                return;
            }
            
            if (tilemap.GetTile(cellPos) != null) return;
            
            _pendingCell = cellPos;
            _hasPendingCell = true;
            
            if (highlighter != null)
            {
                highlighter.gameObject.SetActive(true);
                highlighter.transform.position = tilemap.GetCellCenterWorld(cellPos);
            }
            
            menu.Show();
        }

        private void PositionTooltip(Vector3Int cell)
        {
            if (tooltip == null) return;
            Vector3 worldPos = tilemap.GetCellCenterWorld(cell);
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
            tooltip.transform.position = screenPos;
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
            tilemap.SetTile(_pendingCell, config.Tile);
            
            GameObject towerObj = new GameObject($"Tower_{config.Name}");
            towerObj.transform.position = tilemap.GetCellCenterWorld(_pendingCell);
            
            TowerRuntime runtime = towerObj.AddComponent<TowerRuntime>();
            runtime.Initialize(config, _pendingCell);
            SetupTowerRangeDetection(runtime);

            _placedTowers[_pendingCell] = runtime;
        }
        
        private void SetupTowerRangeDetection(TowerRuntime tower)
        {
            CircleCollider2D rangeTrigger = tower.gameObject.AddComponent<CircleCollider2D>();
            rangeTrigger.radius = tower.Range;
            rangeTrigger.isTrigger = true;
            
            TowerRangeDetector detector = tower.gameObject.AddComponent<TowerRangeDetector>();
            detector.Initialize(tower);
        }

        public void UpgradeTower(TowerRuntime tower)
        {
            int cost = tower.GetUpgradeCost();
            if (credits.TrySpendMoney(cost))
            {
                tower.Level++;
                //TODO
                tower.Damage++;
                tower.Range++;
                tooltip.Show(tower);
                Debug.Log($"Tower upgraded to Level {tower.Level}");
            }
        }

        public void SellTower(TowerRuntime tower)
        {
            int price = tower.GetSellPrice();
            credits.AddMoney(price);
            
            tilemap.SetTile(tower.CellPosition, null);
            _placedTowers.Remove(tower.CellPosition);
            Destroy(tower.gameObject);
            
            Debug.Log($"Tower sold for {price}$");
        }

        private void CancelPlacement()
        {
            _hasPendingCell = false;
            menu.Hide();
            if (highlighter != null) highlighter.gameObject.SetActive(false);
        }
    }
}