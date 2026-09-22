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
        [SerializeField] private GameObject towerPrefab;
        
        private Vector3Int _pendingCell;
        private bool _hasPendingCell;
        private TowerConfig _selectedTower;

        private void Awake()
        {
            menu.TowerChosen += OnTowerChosen;
            menu.Hide();
            if (highlighter != null)
                highlighter.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            menu.TowerChosen -= OnTowerChosen;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
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
                return;
            
            _pendingCell = cellPos;
            _hasPendingCell = true;
            
            ShowHighlighter(cellPos);
            menu.Show();
        }

        private void OnTowerChosen(TowerConfig tower)
        {
            if (!_hasPendingCell) 
                return;
            
            _selectedTower = tower;
            
            if (credits.TrySpendMoney(tower.Cost))
            {
                BuildTower();
            }
            
            CancelPlacement();
        }

        private void BuildTower()
        {
            tilemap.SetTile(_pendingCell, _selectedTower.Tile);
            
            Vector3 worldPos = tilemap.GetCellCenterWorld(_pendingCell);
             var script = Instantiate(towerPrefab, worldPos, Quaternion.identity);
             var comp = script.GetComponent<TowerRuntime>();
             comp.Tower = _selectedTower;
        }

        private void CancelPlacement()
        {
            _hasPendingCell = false;
            _selectedTower = null;
            HideHighlighter();
            menu.Hide();
        }

        private void ShowHighlighter(Vector3Int cell)
        {
            if (highlighter == null) 
                return;
            
            highlighter.gameObject.SetActive(true);
            highlighter.transform.position = tilemap.GetCellCenterWorld(cell);
            
            if (_selectedTower != null)
            {
                bool canAfford = credits.GetMoney() >= _selectedTower.Cost;
                highlighter.color = canAfford ? Color.green : Color.red;
            }
        }

        private void HideHighlighter()
        {
            if (highlighter != null) 
                highlighter.gameObject.SetActive(false);
        }
    }
}