using UnityEngine;
using UnityEngine.UI;

namespace _Source
{
  public class Base : MonoBehaviour
  {
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Image sliderBar;
    private float _currentHealth;

    public float Health => _currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => _currentHealth > 0;

    public System.Action<float, float> OnHealthChanged;
    public System.Action OnBaseDestroyed;

    private void Start()
    {
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
      if (_currentHealth <= 0)
      {
        Destroy();
      }
    }

    private void Destroy()
    {
      OnBaseDestroyed?.Invoke();
      //TODO
      Debug.Log("Game Over! Base destroyed.");
    }

    public void Heal(float amount)
    {
      if (!IsAlive) return;
            
      _currentHealth += amount;
      if (_currentHealth > maxHealth) _currentHealth = maxHealth;
            
      OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }
  }
}