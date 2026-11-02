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

        private float _spriteLocalWidth;         // ширина спрайта в локальных единицах (pivot assumed center)
        private Vector3 _fillInitialLocalScale;  // стартовый масштаб fill
        private Vector3 _fillInitialLocalPos;    // стартовая позиция fill
        private Transform _fillParent;
        private float _currentNormalized = 1f;

        private void Awake()
        {
            if (fill == null || fillRenderer == null) return;

            _fillParent           = fill.parent;
            _fillInitialLocalPos  = fill.localPosition;
            _fillInitialLocalScale = fill.localScale;
            
            _spriteLocalWidth = fillRenderer.sprite != null
                ? fillRenderer.sprite.bounds.size.x
                : 1f;
        }

        public void SetValue(float normalized)
        {
            if (fill == null) return;

            normalized = Mathf.Clamp01(normalized);
            if (Mathf.Approximately(normalized, _currentNormalized)) return;
            _currentNormalized = normalized;
            
            Vector3 scale = _fillInitialLocalScale;
            scale.x = _fillInitialLocalScale.x * normalized;
            fill.localScale = scale;
            float fullWidth = _spriteLocalWidth * _fillInitialLocalScale.x;
            Vector3 pos = _fillInitialLocalPos;
            pos.x = _fillInitialLocalPos.x - fullWidth * (1f - normalized) * 0.5f;
            fill.localPosition = pos;

            if (fillRenderer != null)
            {
                bool visible = true;
                if (hideWhenFull  && normalized >= 0.999f) visible = false;
                if (hideWhenEmpty && normalized <= 0.001f) visible = false;
                fillRenderer.enabled = visible;
            }
        }

        public void Hide()
        {
            if (healthBar != null) healthBar.SetActive(false);
        }
    }
}