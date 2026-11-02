using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TowersSystem.Config;

namespace TowersSystem.Menu
{
    public class TowerBuildButtonTooltip : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private Image icon;

        [Header("Offset")]
        [Tooltip("Смещение тултипа относительно кнопки (в пикселях UI). Y > 0 — выше кнопки.")]
        [SerializeField] private Vector2 offsetFromButton = new Vector2(0f, 10f);

        private RectTransform _panelRect;
        private RectTransform _canvasRect;
        private Canvas _canvas;
        private Camera _uiCamera;

        private void Awake()
        {
            if (panel != null) _panelRect = panel.GetComponent<RectTransform>();

            _canvas = GetComponentInParent<Canvas>();
            if (_canvas != null)
            {
                _canvasRect = _canvas.transform as RectTransform;
                if (_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    _uiCamera = _canvas.worldCamera;
            }

            Hide();
        }
        
        public void Show(TowerConfig config, RectTransform anchor)
        {
            if (panel == null || config == null || anchor == null) return;

            panel.SetActive(true);

            nameText.text    = config.Name;
            description.text = BuildText(config);
            cost.text        = $"Cost: {config.Cost}";
            icon.sprite      = config.Tile != null ? config.Tile.sprite : null;

            PositionAbove(anchor);
        }

        public void Hide()
        {
            if (panel != null) panel.SetActive(false);
        }

        private void PositionAbove(RectTransform anchor) 
        { 
            if (_panelRect == null || _canvasRect == null || anchor == null) return;
            
            Vector3[] corners = new Vector3[4]; 
            anchor.GetWorldCorners(corners); 
            Vector3 topCenterWorld = (corners[1] + corners[2]) * 0.5f; 
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, topCenterWorld);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _canvasRect, screenPoint, _uiCamera, out Vector2 localPoint);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelRect);
            Vector2 panelSize = _panelRect.rect.size; 
            Vector2 pivot     = _panelRect.pivot; 
            Vector2 anchorPt  = new Vector2(0.5f, 0.5f);
            
            Vector2 desired = localPoint
                              + offsetFromButton
                              + new Vector2(-panelSize.x * pivot.x,
                                  panelSize.y * (1f - pivot.y));
            
            Vector2 canvasSize = _canvasRect.rect.size; 
            Vector2 canvasPivot = _canvasRect.pivot; 
            float minX = -canvasSize.x * canvasPivot.x; 
            float maxX =  canvasSize.x * (1f - canvasPivot.x); 
            float minY = -canvasSize.y * canvasPivot.y; 
            float maxY =  canvasSize.y * (1f - canvasPivot.y);
            
            float halfW = panelSize.x; 
            float halfH = panelSize.y; 
            float px = Mathf.Clamp(desired.x,
                           minX + halfW * pivot.x,
                           maxX - halfW * (1f - pivot.x)); 
            float py = Mathf.Clamp(desired.y,
                           minY + halfH * pivot.y,
                           maxY - halfH * (1f - pivot.y));
            
            _panelRect.anchoredPosition = new Vector2(px, py);
}

        private static string BuildText(TowerConfig c)
        {
            switch (c.Type) // отдельный мметод подумать
            {
                case TowerType.Attacker:
                    return "Type: Attacker\n" 
                           + $"Damage: {c.Damage}\n" 
                           + $"Cooldown: {c.Cooldown}s\n";

                case TowerType.Buffer:
                    return "Type: Buffer\n" 
                           + $"Damage Buff: +{c.Buff}%\n" ;

                case TowerType.Slowdown:
                    return "Type: Slowdown\n" 
                           + $"Slow: -{c.Buff}%\n";

                case TowerType.Stun:
                    return "Type: Stun\n"
                           + $"Damage: {c.Damage}\n"
                           + $"Stun: {c.StunDuration}s\n"
                           + $"Cooldown: {c.Cooldown}s\n";
                default:
                    return "Error";
            }
        }
    }
}