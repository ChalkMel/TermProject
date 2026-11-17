using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Ability Config")]
    public class AbilityConfig : ScriptableObject
    {
        [field: SerializeField] public AbilityType Type { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; } = "Ability";

        [Header("Cost / Cooldown")]
        [field: SerializeField] public float ManaCost { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }

        [Header("Damage Over Time")]
        [field: SerializeField] public float DotDamagePerSecond { get; private set; }
        [field: SerializeField] public float DotDuration { get; private set; }

        [Header("Heal")]
        [field: SerializeField] public float HealAmount { get; private set; }
    }
}