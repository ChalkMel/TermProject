using UnityEngine;

namespace LevelSystem.UI
{
    public readonly struct LevelCardViewModel
    {
        public readonly LevelConfig Config;
        public readonly string Title;
        public readonly string Subtitle;
        public readonly Sprite Icon;
        public readonly bool Locked;
        public readonly bool Completed;

        public LevelCardViewModel(LevelConfig config, string title, string subtitle,
            Sprite icon, bool locked, bool completed)
        {
            Config    = config;
            Title     = title;
            Subtitle  = subtitle;
            Icon      = icon;
            Locked    = locked;
            Completed = completed;
        }
    }
}