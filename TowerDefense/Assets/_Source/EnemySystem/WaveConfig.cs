using UnityEngine;

namespace _Source.EnemySystem
{
  [CreateAssetMenu(fileName = "NewWaveConfig", menuName = "Waves/Wave Config")]
  public class WaveConfig : ScriptableObject
  {
    [SerializeField] private EnemyConfig enemy;
    [SerializeField] private int count;
    [SerializeField] private float interval;
    [SerializeField] private float delay;

    public EnemyConfig Enemy => enemy;
    public int Count => count;
    public float Interval => interval;
    public float Delay => delay;
  }
}