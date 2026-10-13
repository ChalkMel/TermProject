using UnityEngine;

namespace EnemySystem.WavesSystem.Config
{
  [CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Enemies/Enemy Config")]
  public class EnemyConfig : ScriptableObject
  {
    [field:SerializeField] public float Speed{get; private set; }
    [field:SerializeField] public int MaxHealth{get; private set; }
    [field:SerializeField] public int Damage{get; private set; }
    [field:SerializeField] public int Reward{get; private set; }
    [field:SerializeField] public GameObject Prefab{get; private set; }
  }
}