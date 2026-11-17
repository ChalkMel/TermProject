using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability Config")]
    public class AbilityConfig : ScriptableObject
    {
        [field: SerializeField] public AbilityType Type { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; } = "Ability";

        [Header("Cost / Cooldown")]
        [field: SerializeField] public float ManaCost { get; private set; } = 30f;
        [field: SerializeField] public float Cooldown { get; private set; } = 5f;

        [Header("Unlock")]
        [Tooltip("Сколько уровней должно быть завершено, чтобы способность открылась")]
        [field: SerializeField] public int RequiredCompletedLevels { get; private set; } = 0;

        [Header("Damage Over Time")]
        [field: SerializeField] public float DotDamagePerSecond { get; private set; } = 3f;
        [field: SerializeField] public float DotDuration { get; private set; } = 5f;

        [Header("Heal")]
        [field: SerializeField] public float HealAmount { get; private set; } = 20f;
    }
}