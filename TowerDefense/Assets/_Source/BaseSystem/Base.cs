using UnityEngine;
using UnityEngine.UI;

namespace BaseSystem
{
  public class Base : MonoBehaviour
  {
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Image sliderBar;
    [SerializeField] private Sprite brokenSprite;
    private float _currentHealth;
    private Sprite _originalSprite;
    private SpriteRenderer _spriteRenderer;
    public float Health => _currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => _currentHealth > 0;

    public System.Action<float, float> OnHealthChanged;
    public System.Action OnBaseDestroyed;

    private void Start()
    {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _originalSprite = _spriteRenderer.sprite;
      _currentHealth = maxHealth;
      sliderBar.fillAmount = _currentHealth / maxHealth;
      OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    public void GetDamage(float damage)
    {
      if (!IsAlive) return;

      _currentHealth -= damage;
      if (_currentHealth < 0) _currentHealth = 0;

      OnHealthChanged?.Invoke(_currentHealth, maxHealth);
      sliderBar.fillAmount = _currentHealth / maxHealth;
      if (_currentHealth <= (maxHealth/2)) ChangeSprite(brokenSprite);
      if (_currentHealth <= 0)
      {
        OnBaseDestroyed?.Invoke();
      }
    }

    public void Heal(float amount)
    {
      if (!IsAlive) return;

      _currentHealth += amount;
      if (_currentHealth > maxHealth) _currentHealth = maxHealth;
      if (_currentHealth <= 50) ChangeSprite(_originalSprite);
      OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    private void ChangeSprite( Sprite sprite)
    {
      _spriteRenderer.sprite = sprite;
    }
  }
}