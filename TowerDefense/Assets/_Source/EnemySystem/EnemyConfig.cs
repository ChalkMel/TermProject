using UnityEngine;

namespace _Source.EnemySystem
{
  [CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Enemies/Enemy Config")]
  public class EnemyConfig : ScriptableObject
  {
    [SerializeField] private float speed;
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float reward;
    [SerializeField] private GameObject prefab;

    public float Speed => speed;
    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public float Reward => reward;
    public GameObject Prefab => prefab;
  }
}