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

        public void Apply(LevelInfoViewModel vm)
        {
            if (levelTitle != null)      levelTitle.text      = vm.Title;
            if (levelSubtitle != null)   levelSubtitle.text   = vm.Subtitle;
            if (difficultyText != null)  difficultyText.text  = vm.DifficultyText;
            if (levelIcon != null && vm.Icon != null) levelIcon.sprite = vm.Icon;
        }
    }
}