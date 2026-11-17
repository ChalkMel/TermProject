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
            levelListView.LevelClicked      += _model.SelectLevel;
            difficultyListView.DifficultyClicked += _model.SelectDifficulty;

            levelListView.Build(levels);
            difficultyListView.Build(difficulties);
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

            infoPanel.Refresh(_model.SelectedLevel, _model.SelectedDifficulty);

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
    }
}