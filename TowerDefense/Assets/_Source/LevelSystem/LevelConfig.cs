using EnemySystem.WavesSystem.Config;
using UnityEngine;

namespace LevelSystem
{
    [CreateAssetMenu(fileName = "NewLevelConfig", menuName = "Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; } = "Level";
        [field: SerializeField] public int LevelNumber { get; private set; } = 1;
        [field: SerializeField] public string SceneName { get; private set; }

        [Header("Economy")]
        [field: SerializeField] public int StartingCredits { get; private set; } = 120;

        [Header("Defense")]
        [field: SerializeField] public float BaseHealth { get; private set; } = 100f;
        
        [field: SerializeField] public WaveGroups WaveGroups { get; private set; }
        [field: SerializeField] public LevelConfig NextLevel { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite LevelIcon { get; private set; }
        
    }
}