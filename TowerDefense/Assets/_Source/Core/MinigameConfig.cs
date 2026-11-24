namespace Core
{
    using UnityEngine;

    namespace Core
    {
        [CreateAssetMenu(fileName = "NewMinigame", menuName = "Minigames/Minigame Config")]
        public class MinigameConfig : ScriptableObject
        {
            [field: SerializeField] public string SceneName   { get; private set; }
            [field: SerializeField] public string DisplayName { get; private set; }
        }
    }
}