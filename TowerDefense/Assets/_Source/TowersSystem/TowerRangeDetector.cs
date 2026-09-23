using UnityEngine;
using _Source.EnemySystem;

namespace _Source.TowersSystem
{
  [RequireComponent(typeof(CircleCollider2D))]
  public class TowerRangeDetector : MonoBehaviour
  {
    private TowerRuntimeBase _tower;

    public void Initialize(TowerRuntimeBase tower) => _tower = tower;

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (_tower == null) return;

      var enemy = other.GetComponent<EnemyRuntime>();
      if (enemy != null) { _tower.OnEnemyEnteredRange(enemy); return; }

      var otherTower = other.GetComponent<TowerRuntimeBase>();
      if (otherTower != null && otherTower != _tower)
        _tower.OnTowerEnteredRange(otherTower);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (_tower == null) return;

      var enemy = other.GetComponent<EnemyRuntime>();
      if (enemy != null) { _tower.OnEnemyExitedRange(enemy); return; }

      var otherTower = other.GetComponent<TowerRuntimeBase>();
      if (otherTower != null && otherTower != _tower)
        _tower.OnTowerExitedRange(otherTower);
    }
  }
}