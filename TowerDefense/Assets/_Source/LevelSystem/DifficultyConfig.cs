using UnityEngine;

namespace LevelSystem
{
    [CreateAssetMenu(fileName = "NewDifficulty", menuName = "Game/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; } = "Normal";

        [Header("Economy")]
        [Tooltip("Множитель стартовых кредитов.")]
        [field: SerializeField] public float StartingCreditsMultiplier { get; private set; } = 1f;
        [Tooltip("Множитель награды за убийство врага.")]
        [field: SerializeField] public float EnemyRewardMultiplier { get; private set; } = 1f;
        [Tooltip("Множитель награды за жёлудь.")]
        [field: SerializeField] public float AcornRewardMultiplier { get; private set; } = 1f;

        [Header("Enemies")]
        [field: SerializeField] public float EnemyHealthMultiplier { get; private set; } = 1f;
        [field: SerializeField] public float EnemyDamageMultiplier { get; private set; } = 1f;
        [field: SerializeField] public float EnemySpeedMultiplier  { get; private set; } = 1f;

        [Header("Pacing")]
        [Tooltip(">1 — медленнее волны (легче), <1 — быстрее.")]
        [field: SerializeField] public float WavePacingMultiplier { get; private set; } = 1f;
        [Tooltip(">1 — жёлуди реже (сложнее).")]
        [field: SerializeField] public float AcornSpawnIntervalMultiplier { get; private set; } = 1f;

        [Header("Defense")]
        [field: SerializeField] public float BaseHealthMultiplier { get; private set; } = 1f;

        [Header("Heat (Tea level)")]
        [Tooltip(">1 — быстрее остывает (сложнее).")]
        [field: SerializeField] public float HeatDecayMultiplier { get; private set; } = 1f;
        [field: SerializeField] public float HeatPerPressMultiplier { get; private set; } = 1f;
        [field: SerializeField] public float OverheatDamageMultiplier { get; private set; } = 1f;
    }
}