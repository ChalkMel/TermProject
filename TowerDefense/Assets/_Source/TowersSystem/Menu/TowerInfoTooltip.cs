using TMPro;
using UnityEngine;

namespace _Source.TowersSystem.Menu
{
  public class TowerInfoTooltip : MonoBehaviour
  {
    [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Vector3 cursorOffset = new Vector3(20f, -20f, 0f);

        private RectTransform _panelRect;
        private Canvas _canvas;
        private Camera _uiCamera;

        private void Awake()
        {
            if (panel != null) _panelRect = panel.GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();

            if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                _uiCamera = _canvas.worldCamera;

            Hide();
        }

        public void Show(TowerConfig config, Vector2 screenPosition)
        {
            if (panel == null || config == null) return;

            panel.SetActive(true);
            text.text = BuildText(config);
            Follow(screenPosition);
        }

        public void Follow(Vector2 screenPosition)
        {
            if (_panelRect == null || _canvas == null) return;

            Vector2 local;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    screenPosition,
                    _uiCamera,
                    out local))
            {
                _panelRect.localPosition = local + (Vector2)cursorOffset;
            }
        }

        public void Hide()
        {
            if (panel != null) panel.SetActive(false);
        }

        private static string BuildText(TowerConfig c)
        {
            switch (c.Type)
            {
                case TowerType.Attacker:
                    return $"<b>{c.Name}</b>\n" +
                           $"Type: Attacker\n" +
                           $"Damage: {c.Damage}\n" +
                           $"Cooldown: {c.Cooldown}s\n" +
                           $"Range: {c.Range}\n" +
                           $"Cost: {c.Cost}$";

                case TowerType.Buffer:
                    return $"<b>{c.Name}</b>\n" +
                           $"Type: Buffer\n" +
                           $"Damage Buff: +{c.Buff}%\n" +
                           $"Range: {c.Range}\n" +
                           $"Cost: {c.Cost}$";

                case TowerType.Slowdown:
                    return $"<b>{c.Name}</b>\n" +
                           $"Type: Slowdown\n" +
                           $"Slow: -{c.Buff}%\n" +
                           $"Range: {c.Range}\n" +
                           $"Cost: {c.Cost}$";

                default:
                    return $"<b>{c.Name}</b>\nCost: {c.Cost}$";
            }
        }
    }
}