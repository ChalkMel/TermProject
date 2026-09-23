using UnityEngine;

namespace _Source.EnemySystem
{
  [CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Enemies/Enemy Config")]
  public class EnemyConfig : ScriptableObject
  {
    [SerializeField] private float speed;
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private int reward;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float attackInterval = 2f;
    public float AttackInterval => attackInterval;

    public float Speed => speed;
    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public int Reward => reward;
    public GameObject Prefab => prefab;
  }
}