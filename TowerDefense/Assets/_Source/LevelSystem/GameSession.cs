using UnityEngine;

namespace LevelSystem
{
    public static class GameSession
    {
        public static LevelConfig CurrentLevel { get; private set; }
        public static DifficultyConfig CurrentDifficulty { get; private set; }

        public static bool HasSelection => CurrentLevel != null && CurrentDifficulty != null;

        public static void Select(LevelConfig level, DifficultyConfig difficulty)
        {
            CurrentLevel = level;
            CurrentDifficulty = difficulty;
        }

        public static void Clear()
        {
            CurrentLevel = null;
            CurrentDifficulty = null;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => Clear();
    }
}