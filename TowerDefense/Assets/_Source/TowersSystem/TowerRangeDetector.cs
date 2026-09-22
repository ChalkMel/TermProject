using UnityEngine;
using _Source.EnemySystem;
using _Source.TowersSystem;

namespace _Source.TowersSystem
{
  [RequireComponent(typeof(CircleCollider2D))]
  public class TowerRangeDetector : MonoBehaviour
  {
    private TowerRuntime _tower;

    public void Initialize(TowerRuntime tower)
    {
      _tower = tower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      Debug.Log("Enemy caught");
      EnemyRuntime enemy = collision.GetComponent<EnemyRuntime>();
      if (enemy != null && _tower != null)
      {
        _tower.OnEnemyEnteredRange(enemy);
      }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
      EnemyRuntime enemy = collision.GetComponent<EnemyRuntime>();
      if (enemy != null && _tower != null)
      {
        _tower.OnEnemyExitedRange(enemy);
      }
    }
  }
}