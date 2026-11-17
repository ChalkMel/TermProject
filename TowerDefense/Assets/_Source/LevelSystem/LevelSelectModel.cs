using System;

namespace LevelSystem
{
    public class LevelSelectModel
    {
        public LevelConfig SelectedLevel { get; private set; }
        public DifficultyConfig SelectedDifficulty { get; private set; }

        public event Action SelectionChanged;

        public bool CanStart =>
            SelectedLevel != null && SelectedDifficulty != null;

        public void SelectLevel(LevelConfig level)
        {
            if (level == SelectedLevel) return;
            SelectedLevel = level;
            SelectionChanged?.Invoke();
        }

        public void SelectDifficulty(DifficultyConfig difficulty)
        {
            if (difficulty == SelectedDifficulty) return;
            SelectedDifficulty = difficulty;
            SelectionChanged?.Invoke();
        }
    }
}