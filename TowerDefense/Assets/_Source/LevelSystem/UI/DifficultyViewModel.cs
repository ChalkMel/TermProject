namespace LevelSystem.UI
{
    public readonly struct DifficultyViewModel
    {
        public readonly DifficultyConfig Config;
        public readonly string Label;
        public readonly string Description;

        public DifficultyViewModel(DifficultyConfig config, string label, string description)
        {
            Config      = config;
            Label       = label;
            Description = description;
        }
    }
}