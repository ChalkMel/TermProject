using UnityEngine;

namespace _Source.TowersSystem
{
  public class TowerRuntime : MonoBehaviour
  {
    [SerializeField] private TowerConfig tower;
        
    private float _currentCooldown;
    private float _currentDamage;
    private float _currentRange;
    private int _currentLevel;
        
    public TowerConfig Tower
    {
      get => tower;
      set => tower = value;
    }

    public float CurrentCooldown => _currentCooldown;
    public float CurrentDamage => _currentDamage;
    public float CurrentRange => _currentRange;
    public int CurrentLevel => _currentLevel;

    private void Awake()
    {
      if (tower != null)
      {
        _currentCooldown = tower.Cooldown;
        _currentDamage = tower.Damage;
        _currentRange = tower.Range;
        _currentLevel = tower.Level;
      }
    }

    public void Shoot()
    {
      Debug.Log($"Tower shooting! Damage: {_currentDamage}, Range: {_currentRange}");
    }
  }
}