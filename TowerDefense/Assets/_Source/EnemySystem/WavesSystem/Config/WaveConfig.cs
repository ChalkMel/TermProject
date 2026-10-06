using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemySystem.WavesSystem.Config
{
  [CreateAssetMenu(fileName = "NewWaveConfig", menuName = "Waves/Wave Config")]
  public class WaveConfig : ScriptableObject
  {
    [field:SerializeField] public List<EnemyGroup> groups = new List<EnemyGroup>();
    [field:SerializeField] public float delayBeforeWave = 2f;

    public List<EnemyGroup> Groups => groups;
    public float Delay => delayBeforeWave;
  }

  [System.Serializable]
    public class EnemyGroup //сеттеры
    {
      [field:SerializeField] public EnemyConfig Enemy { get; private set; }
      [field:SerializeField] public int Count{ get; private set; }
      [field:SerializeField] public float Interval { get; private set; }
    }
  }
