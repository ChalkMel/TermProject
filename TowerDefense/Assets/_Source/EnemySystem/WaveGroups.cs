using UnityEngine;
using System.Collections.Generic;
using _Source.EnemySystem;

namespace _Source.Waves
{
  [CreateAssetMenu(fileName = "NewWaveGroups", menuName = "Waves/Wave Groups")]
  public class WaveGroups : ScriptableObject
  {
    [SerializeField] private List<WaveConfig> waves = new List<WaveConfig>();
        
    public List<WaveConfig> Waves => waves;
    public int WaveCount => waves.Count;
        
    public WaveConfig GetWave(int index)
    {
      if (index >= 0 && index < waves.Count)
        return waves[index];
      return null;
    }
  }
}