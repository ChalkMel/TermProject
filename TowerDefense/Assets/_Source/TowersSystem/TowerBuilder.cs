using System.Collections.Generic;
using _Source.Resources;
using _Source.TowersSystem.Menu;
using _Source.TowersSystem.Runtimes;
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

        [Header("Range Colors")]
        [SerializeField] private Color placedRangeColor  = new Color(1f, 1f, 0f, 0.6f);
        [SerializeField] private Color previewRangeColor = new Color(0.2f, 0.8f, 1f, 0.6f);

        private readonly Dictionary<Vector3Int, TowerRuntimeBase> _placedTowers = new();

        private Vector3Int _pendingCell;
        private bool _hasPendingCell;

        private RangeIndicator _placedRangeIndicator;
        private RangeIndicator _previewRangeIndicator;
        private SpriteRenderer _previewSprite;

        private void Awake()
        {
            menu.TowerChosen      += OnTowerChosen;
            menu.TowerHoverStarted += OnTowerHoverStarted;
            menu.TowerHoverEnded   += OnTowerHoverEnded;
            menu.Hide();

            if (tooltip != null)
            {
                tooltip.SetBuilder(this);
                tooltip.OnHidden += OnTooltipHidden;
                tooltip.Hide();
            }

            if (highlighter != null) highlighter.gameObject.SetActive(false);

            CreateIndicators();
        }

        private void OnDestroy()
        {
            menu.TowerChosen      -= OnTowerChosen;
            menu.TowerHoverStarted -= OnTowerHoverStarted;
            menu.TowerHoverEnded   -= OnTowerHoverEnded;
            if (tooltip != null) tooltip.OnHidden -= OnTooltipHidden;
        }

        private void CreateIndicators()
        {
            var placedGo = new GameObject("PlacedRangeIndicator");
            placedGo.transform.SetParent(transform, false);
            _placedRangeIndicator = placedGo.AddComponent<RangeIndicator>();
            _placedRangeIndicator.SetColor(placedRangeColor);

            var previewRangeGo = new GameObject("PreviewRangeIndicator");
            previewRangeGo.transform.SetParent(transform, false);
            _previewRangeIndicator = previewRangeGo.AddComponent<RangeIndicator>();
            _previewRangeIndicator.SetColor(previewRangeColor);

            var previewSpriteGo = new GameObject("PreviewTowerSprite");
            previewSpriteGo.transform.SetParent(transform, false);
            _previewSprite = previewSpriteGo.AddComponent<SpriteRenderer>();
            _previewSprite.color = new Color(1f, 1f, 1f, 0.6f);
            _previewSprite.sortingOrder = 99;
            _previewSprite.enabled = false;
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
            if (_placedTowers.TryGetValue(cellPos, out TowerRuntimeBase existing))
            {
                HidePreview();
                tooltip.Show(existing);
                PositionTooltip(cellPos);
                _placedRangeIndicator.Show(existing.transform.position, existing.Range);
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
                PlaceTower(tower);

            CancelPlacement();
        }

        private void OnTowerHoverStarted(TowerConfig config)
        {
            if (!_hasPendingCell || config == null) return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(_pendingCell);

            _previewRangeIndicator.Show(worldPos, config.Range);

            if (config.Tile != null && config.Tile.sprite != null)
            {
                _previewSprite.sprite = config.Tile.sprite;
                _previewSprite.transform.position = worldPos;
                _previewSprite.enabled = true;
                
                Vector3 cellSize = tilemap.cellSize;
                Sprite s = config.Tile.sprite;
                float targetSizeX = cellSize.x;
                float targetSizeY = cellSize.y;
                
                float spriteSizeX = s.bounds.size.x;
                float spriteSizeY = s.bounds.size.y;

                float scaleX = spriteSizeX > 0 ? targetSizeX / spriteSizeX : 1f;
                float scaleY = spriteSizeY > 0 ? targetSizeY / spriteSizeY : 1f;

                _previewSprite.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
            
            if (highlighter != null) highlighter.gameObject.SetActive(false);
        }

        private void OnTowerHoverEnded(TowerConfig config)
        {
            HidePreview();
            if (_hasPendingCell && highlighter != null)
            {
                highlighter.gameObject.SetActive(true);
                highlighter.transform.position = tilemap.GetCellCenterWorld(_pendingCell);
            }
        }

        private void HidePreview()
        {
            if (_previewRangeIndicator != null) _previewRangeIndicator.Hide();
            if (_previewSprite != null) _previewSprite.enabled = false;
        }

        private void OnTooltipHidden()
        {
            if (_placedRangeIndicator != null) _placedRangeIndicator.Hide();
        }
        

        private void PlaceTower(TowerConfig config)
        {
            tilemap.SetTile(_pendingCell, config.Tile);

            var go = new GameObject($"Tower_{config.Name}");
            go.transform.position = tilemap.GetCellCenterWorld(_pendingCell);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            var runtime = AddTowerComponent(go, config.Type);
            runtime.Initialize(config, _pendingCell);

            SetupTowerRangeDetection(runtime);
            _placedTowers[_pendingCell] = runtime;
        }

        private static TowerRuntimeBase AddTowerComponent(GameObject go, TowerType type)
        {
            return type switch
            {
                TowerType.Attacker => go.AddComponent<AttackerTowerRuntime>(),
                TowerType.Buffer   => go.AddComponent<BufferTowerRuntime>(),
                TowerType.Slowdown => go.AddComponent<SlowdownTowerRuntime>(),
                _                  => go.AddComponent<AttackerTowerRuntime>()
            };
        }

        private void SetupTowerRangeDetection(TowerRuntimeBase tower)
        {
            var rangeTrigger = tower.gameObject.AddComponent<CircleCollider2D>();
            rangeTrigger.radius = tower.Range;
            rangeTrigger.isTrigger = true;

            var detector = tower.gameObject.AddComponent<TowerRangeDetector>();
            detector.Initialize(tower);
        }

        public void UpgradeTower(TowerRuntimeBase tower)
        {
            int cost = tower.GetUpgradeCost();
            if (!credits.TrySpendMoney(cost)) return;

            tower.Upgrade();
            tooltip.Show(tower);
            
            _placedRangeIndicator.Show(tower.transform.position, tower.Range);

            Debug.Log($"Tower upgraded to Level {tower.Level}");
        }

        public void SellTower(TowerRuntimeBase tower)
        {
            int price = tower.GetSellPrice();
            credits.AddMoney(price);

            tower.OnBeforeDestroy();
            tilemap.SetTile(tower.CellPosition, null);
            _placedTowers.Remove(tower.CellPosition);
            Destroy(tower.gameObject);

            _placedRangeIndicator.Hide();
            Debug.Log($"Tower sold for {price}");
        }

        private void CancelPlacement()
        {
            _hasPendingCell = false;
            menu.Hide();
            HidePreview();
            if (highlighter != null) highlighter.gameObject.SetActive(false);
        }
    }
}