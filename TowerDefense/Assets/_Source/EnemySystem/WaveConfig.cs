using System.Collections.Generic;
using UnityEngine;

namespace _Source.EnemySystem
{
  [CreateAssetMenu(fileName = "NewWaveConfig", menuName = "Waves/Wave Config")]
  public class WaveConfig : ScriptableObject
  {
    [SerializeField] private List<EnemyGroup> groups = new List<EnemyGroup>();
    [SerializeField] private float delayBeforeWave = 2f;

    public List<EnemyGroup> Groups => groups;
    public float Delay => delayBeforeWave;
  }

  [System.Serializable]
    public class EnemyGroup
    {
      public EnemyConfig enemy;
      public int count = 1;
      public float interval = 1f;
    }
  }
