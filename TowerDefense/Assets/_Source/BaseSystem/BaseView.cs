using UnityEngine;
using UnityEngine.UI;

namespace BaseSystem
{
    public class BaseView : MonoBehaviour
    {
        [SerializeField] private Base baseRef;
        [SerializeField] private Image healthBar;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite brokenSprite;
        [Tooltip("Порог, ниже которого показывается brokenSprite (доля от максимума)")]
        [SerializeField, Range(0f, 1f)] private float brokenThreshold = 0.5f;

        private bool _isBroken;

        private void Awake()
        {
            if (baseRef == null) baseRef = GetComponent<Base>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (normalSprite == null && spriteRenderer != null) normalSprite = spriteRenderer.sprite;
        }

        private void OnEnable()
        {
            if (baseRef == null) return;
            baseRef.OnHealthChanged += Draw;
            Draw(baseRef.Health, baseRef.MaxHealth);
        }

        private void OnDisable()
        {
            if (baseRef == null) return;
            baseRef.OnHealthChanged -= Draw;
        }

        private void Draw(float current, float max)
        {
            if (healthBar != null)
                healthBar.fillAmount = max > 0f ? current / max : 0f;

            if (spriteRenderer == null || normalSprite == null || brokenSprite == null)
                return;

            bool shouldBeBroken = max > 0f && (current / max) <= brokenThreshold;
            if (shouldBeBroken == _isBroken) return;

            _isBroken = shouldBeBroken;
            spriteRenderer.sprite = _isBroken ? brokenSprite : normalSprite;
        }
    }
}