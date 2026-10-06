using UnityEngine;

namespace EnemySystem
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Transform fill;
        [SerializeField] private SpriteRenderer fillRenderer;
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private bool hideWhenEmpty;
        [SerializeField] private GameObject healthBar;
        private float _maxFillWidth;
        private Vector3 _fillInitialLocalPos;
        private Transform _fillParent;
        private float _currentNormalized = 1f;

        private void Awake()
        {
            if (fill == null || fillRenderer == null) return;

            _fillParent = fill.parent;
            _fillInitialLocalPos = fill.localPosition;
            _maxFillWidth = fillRenderer.bounds.size.x > 0
                ? fillRenderer.bounds.size.x
                : fill.localScale.x;
        }
        public void SetValue(float normalized)
        {
            if (fill == null) return;

            normalized = Mathf.Clamp01(normalized);
            if (Mathf.Approximately(normalized, _currentNormalized)) return;
            _currentNormalized = normalized;
            
            Vector3 scale = fill.localScale;
            scale.x = _maxFillWidth * normalized;
            fill.localScale = scale;
            
            Vector3 pos = _fillInitialLocalPos;
            pos.x = _fillInitialLocalPos.x - (_maxFillWidth * (1f - normalized)) * 0.5f;
            fill.localPosition = pos;
            
            if (fillRenderer != null)
            {
                bool visible = true;
                if (hideWhenFull && normalized >= 0.999f) visible = false;
                if (hideWhenEmpty && normalized <= 0.001f) visible = false;
                fillRenderer.enabled = visible;
            }
        }

        public void Hide()
        {
            healthBar.SetActive(false);
        }
    }
}