using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LevelSystem.UI
{
    public class LevelSelectController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private List<LevelConfig> levels = new();
        [SerializeField] private List<DifficultyConfig> difficulties = new();
        [SerializeField] private DifficultyConfig defaultDifficulty;

        [Header("Views")]
        [SerializeField] private LevelListView levelListView;
        [SerializeField] private DifficultyListView difficultyListView;
        [SerializeField] private LevelInfoPanel infoPanel;

        [Header("Actions")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button backButton;

        [Header("Navigation")]
        [SerializeField] private string backSceneName = "MainMenu";

        private readonly LevelSelectModel _model = new();
        

        private void Start()
        {
            BuildViews();
            SetupActions();

            _model.SelectionChanged += OnSelectionChanged;

            SelectDefaults();
        }

        private void OnDestroy()
        {
            _model.SelectionChanged -= OnSelectionChanged;
        }
        

        private void BuildViews()
        {
            levelListView.LevelClicked          += _model.SelectLevel;
            difficultyListView.DifficultyClicked += _model.SelectDifficulty;

            levelListView.Build(BuildLevelCards(levels));
            difficultyListView.Build(BuildDifficultyCards(difficulties));
        }

        private void SetupActions()
        {
            if (startButton != null) startButton.onClick.AddListener(StartGame);
            if (backButton  != null) backButton.onClick.AddListener(GoBack);
        }

        private void SelectDefaults()
        {
            var diff = defaultDifficulty != null
                ? defaultDifficulty
                : (difficulties.Count > 0 ? difficulties[0] : null);

            if (diff != null) _model.SelectDifficulty(diff);

            int firstUnlocked = LevelProgress.FirstUnlockedIndex(levels);
            if (firstUnlocked >= 0) _model.SelectLevel(levels[firstUnlocked]);
            OnSelectionChanged();
        }


        private void OnSelectionChanged()
        {
            levelListView.SetSelected(_model.SelectedLevel);
            difficultyListView.SetSelected(_model.SelectedDifficulty);

            infoPanel.Apply(BuildInfo(_model.SelectedLevel, _model.SelectedDifficulty));

            if (startButton != null)
                startButton.interactable = _model.CanStart
                    && LevelProgress.IsUnlocked(_model.SelectedLevel, levels);
        }

        private void StartGame()
        {
            if (!_model.CanStart) return;
            if (!LevelProgress.IsUnlocked(_model.SelectedLevel, levels)) return;

            if (string.IsNullOrEmpty(_model.SelectedLevel.SceneName))
            {
                Debug.LogError($"У уровня '{_model.SelectedLevel.name}' не задан SceneName.");
                return;
            }

            GameSession.Select(_model.SelectedLevel, _model.SelectedDifficulty);
            SceneManager.LoadScene(_model.SelectedLevel.SceneName);
        }

        private void GoBack()
        {
            if (!string.IsNullOrEmpty(backSceneName))
                SceneManager.LoadScene(backSceneName);
        }

        private List<LevelCardViewModel> BuildLevelCards(IList<LevelConfig> ordered)
        {
            var result = new List<LevelCardViewModel>(ordered?.Count ?? 0);
            if (ordered == null) return result;

            foreach (var lvl in ordered)
            {
                if (lvl == null) continue;
                result.Add(BuildLevelCard(lvl, ordered));
            }
            return result;
        }

        private static LevelCardViewModel BuildLevelCard(LevelConfig level, IList<LevelConfig> ordered)
        {
            bool locked    = !LevelProgress.IsUnlocked(level, ordered);
            bool completed =  LevelProgress.IsCompleted(level);

            string title = string.IsNullOrEmpty(level.DisplayName)
                ? $"Level {level.LevelNumber}"
                : level.DisplayName;

            string subtitle = locked
                ? "Locked"
                : (completed ? "Completed" : $"Level {level.LevelNumber}");

            return new LevelCardViewModel(
                config:    level,
                title:     title,
                subtitle:  subtitle,
                icon:      level.LevelIcon,
                locked:    locked,
                completed: completed);
        }

        private static List<DifficultyViewModel> BuildDifficultyCards(IList<DifficultyConfig> source)
        {
            var result = new List<DifficultyViewModel>(source?.Count ?? 0);
            if (source == null) return result;

            foreach (var d in source)
            {
                if (d == null) continue;
                result.Add(BuildDifficultyCard(d));
            }
            return result;
        }

        private static DifficultyViewModel BuildDifficultyCard(DifficultyConfig d)
        {
            string label = !string.IsNullOrEmpty(d.DisplayName) ? d.DisplayName : d.name;
            return new DifficultyViewModel(d, label, BuildAutoDescription(d));
        }

        private static LevelInfoViewModel BuildInfo(LevelConfig level, DifficultyConfig diff)
        {
            if (level == null)
                return new LevelInfoViewModel("—", string.Empty, "Difficulty: —", null);

            string title = string.IsNullOrEmpty(level.DisplayName)
                ? $"Level {level.LevelNumber}"
                : level.DisplayName;

            string subtitle = diff == null
                ? string.Empty
                : $"Starting credits: {level.StartingCredits * diff.StartingCreditsMultiplier}    " +
                  $"•    Base HP: {level.BaseHealth * diff.BaseHealthMultiplier}";

            string diffText = diff != null
                ? $"Difficulty: <b>{diff.DisplayName}</b>"
                : "Difficulty: —";

            return new LevelInfoViewModel(title, subtitle, diffText, level.LevelIcon);
        }
        private static string BuildAutoDescription(DifficultyConfig d)
        {
            var sb = new System.Text.StringBuilder();

            void Add(string label, float v, bool percentIncrease)
            {
                if (Mathf.Approximately(v, 1f)) return;
                float pct = percentIncrease ? (v - 1f) * 100f : -(v - 1f) * 100f;
                string sign = pct >= 0f ? "+" : "";
                sb.AppendLine($"{label}: {sign}{pct:F0}%");
            }

            Add("Enemy HP",   d.EnemyHealthMultiplier, true);
            Add("Enemy DMG",  d.EnemyDamageMultiplier, true);
            Add("Enemy SPD",  d.EnemySpeedMultiplier,  true);
            Add("Rewards",    d.EnemyRewardMultiplier, true);
            Add("Base HP",    d.BaseHealthMultiplier,  true);

            return sb.ToString().TrimEnd();
        }
    }
}