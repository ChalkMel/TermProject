using UnityEngine;

namespace LevelSystem.UI
{
    public readonly struct LevelInfoViewModel
    {
        public readonly string Title;
        public readonly string Subtitle;
        public readonly string DifficultyText;
        public readonly Sprite Icon;

        public LevelInfoViewModel(string title, string subtitle, string diffText, Sprite icon)
        {
            Title          = title;
            Subtitle       = subtitle;
            DifficultyText = diffText;
            Icon           = icon;
        }
    }
}