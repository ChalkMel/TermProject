using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemySystem.WavesSystem.Config
{
  [CreateAssetMenu(fileName = "NewWaveGroups", menuName = "Waves/Wave Groups")]
  public class WaveGroups : ScriptableObject
  {
    [field:SerializeField]public List<WaveConfig> Waves {get; private set;} = new List<WaveConfig>();
    public int WaveCount  => Waves.Count; //Разберись
        
    public WaveConfig GetWave(int index)
    {
      if (index >= 0 && index < Waves.Count)
        return Waves[index];
      return null;
    }
  }
}