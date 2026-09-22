// TowerBuilder.cs (улучшенная версия)
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
        [SerializeField] private Credits credits;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Camera cam;
        [SerializeField] private SpriteRenderer highlighter;
        [SerializeField] private LayerMask enemyLayer;
        
        private Vector3Int _pendingCell;
        private bool _hasPendingCell;
        private TowerConfig _selectedTower;

        public System.Action<Vector3Int, TowerConfig> OnTowerPlaced;
        public System.Action OnPlacementCancelled;

        private void Awake()
        {
            if (menu != null)
            {
                menu.TowerChosen += OnTowerChosen;
                menu.Hide();
            }
            
            if (highlighter != null)
                highlighter.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (menu != null)
                menu.TowerChosen -= OnTowerChosen;
        }

        private void Update()
        {
            HandleInput();
            UpdateHighlighter();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (_hasPendingCell)
                {
                    CancelPlacement();
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
                
                TryOpenMenuForCell(cellPos);
            }
        }

        private void TryOpenMenuForCell(Vector3Int cellPos)
        {
            if (tilemap.GetTile(cellPos) != null) 
            {
                return;
            }

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

            _selectedTower = tower;
            
            if (credits.TrySpendMoney(tower.Cost))
            {
                PlaceTower();
            }
            else
            {
                Debug.LogWarning("Not enough money!");
                CancelPlacement();
            }
        }

        private void PlaceTower()
        {
            if (_selectedTower == null || _selectedTower.Tile == null) return;
            
            tilemap.SetTile(_pendingCell, _selectedTower.Tile);
            
            Vector3 worldPos = tilemap.GetCellCenterWorld(_pendingCell);
            GameObject towerObj = new GameObject($"Tower_{_selectedTower.Name}");
            towerObj.transform.position = worldPos;
            
            TowerRuntime runtime = towerObj.AddComponent<TowerRuntime>();
            runtime.Initialize(_selectedTower);
            
            SetupTowerRangeDetection(runtime);
            
            OnTowerPlaced?.Invoke(_pendingCell, _selectedTower);
            
            CancelPlacement();
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
            _selectedTower = null;
            HideHighlighter();
            
            if (menu != null)
                menu.Hide();
            
            OnPlacementCancelled?.Invoke();
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

        private void UpdateHighlighter()
        {
            if (!_hasPendingCell || highlighter == null || !highlighter.gameObject.activeSelf)
                return;
            
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);
            
            if (cellPos != _pendingCell)
            {
                // TODO: update highlighter to follow mouse
            }
        }

        public void SetSelectedTower(TowerConfig tower)
        {
            _selectedTower = tower;
        }
    }
}