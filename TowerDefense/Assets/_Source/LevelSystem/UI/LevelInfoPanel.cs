using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSystem.UI
{
    public class LevelInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelTitle;
        [SerializeField] private TextMeshProUGUI levelSubtitle;
        [SerializeField] private TextMeshProUGUI difficultyText;
        [SerializeField] private Image levelIcon;

        public void Refresh(LevelConfig level, DifficultyConfig difficulty)
        {
            if (levelTitle != null)
                levelTitle.text = level != null
                    ? (string.IsNullOrEmpty(level.DisplayName)
                        ? $"Level {level.LevelNumber}"
                        : level.DisplayName)
                    : "—";

            if (levelSubtitle != null)
            {
                levelSubtitle.text = (level == null || difficulty == null)
                    ? string.Empty
                    : $"Starting credits: {level.StartingCredits * difficulty.StartingCreditsMultiplier}    " +
                      $"•    Base HP: {level.BaseHealth * difficulty.BaseHealthMultiplier}";
            }

            if (difficultyText != null)
                difficultyText.text = difficulty != null
                    ? $"Difficulty: <b>{difficulty.DisplayName}</b>"
                    : "Difficulty: —";

            if (levelIcon != null && level != null)
                levelIcon.sprite = level.LevelIcon;
        }
    }
}