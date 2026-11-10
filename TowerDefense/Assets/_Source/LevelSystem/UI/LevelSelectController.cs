using System.Collections.Generic;
using TMPro;
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

    [Header("Level list")]
    [SerializeField] private Transform levelListContainer;
    [SerializeField] private LevelCardButton levelCardPrefab;

    [Header("Difficulty list")]
    [SerializeField] private Transform difficultyContainer;
    [SerializeField] private DifficultyButton difficultyButtonPrefab;

    [Header("Info panel")]
    [SerializeField] private TextMeshProUGUI selectedLevelTitle;
    [SerializeField] private TextMeshProUGUI selectedLevelSubtitle;
    [SerializeField] private TextMeshProUGUI selectedDifficultyText;
    [SerializeField] private Image selectedLevelIcon;

    [Header("Actions")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;

    [Header("Navigation")]
    [SerializeField] private string backSceneName = "MainMenu";

    private readonly List<LevelCardButton> _levelButtons = new();
    private readonly List<DifficultyButton> _difficultyButtons = new();

    private LevelConfig _selectedLevel;
    private DifficultyConfig _selectedDifficulty;

    private void Start()
    {
      BuildLevelList();
      BuildDifficultyList();
      SetupActions();
      
      DifficultyConfig defaultDiff = defaultDifficulty;
            if (defaultDiff == null && difficulties.Count > 0) defaultDiff = difficulties[0];
            if (defaultDiff != null) SelectDifficulty(defaultDiff);
            
      int firstUnlocked = LevelProgress.FirstUnlockedIndex(levels);
      if (firstUnlocked >= 0) SelectLevel(levels[firstUnlocked]);
      
      RefreshInfoPanel();
    }

    private void BuildLevelList()
    {
      if (levelCardPrefab == null || levelListContainer == null) return;

      foreach (var cfg in levels)
      {
        if (cfg == null) continue;

        bool locked    = !LevelProgress.IsUnlocked(cfg, levels);
        bool completed =  LevelProgress.IsCompleted(cfg);

        var card = Instantiate(levelCardPrefab, levelListContainer);
        card.Bind(cfg, locked, completed);
        card.OnClick += _ => SelectLevel(cfg);
        _levelButtons.Add(card);
      }
    }

    private void BuildDifficultyList()
    {
      if (difficultyButtonPrefab == null || difficultyContainer == null) return;

      foreach (var cfg in difficulties)
      {
        if (cfg == null) continue;
        var btn = Instantiate(difficultyButtonPrefab, difficultyContainer);
        btn.Bind(cfg);
        btn.OnClick += _ => SelectDifficulty(cfg);
        _difficultyButtons.Add(btn);
      }
    }

    private void SetupActions()
    {
      if (startButton != null) startButton.onClick.AddListener(StartGame);
      if (backButton  != null) backButton.onClick.AddListener(GoBack);
    }

    public void SelectLevel(LevelConfig level)
    {
      if (level == null) return;
      if (!LevelProgress.IsUnlocked(level, levels)) return;

      _selectedLevel = level;

      foreach (var b in _levelButtons)
        b.SetSelected(b.Config == level);

      RefreshInfoPanel();
    }

    public void SelectDifficulty(DifficultyConfig difficulty)
    {
      if (difficulty == null) return;
      _selectedDifficulty = difficulty;

      foreach (var b in _difficultyButtons)
        b.SetSelected(b.Config == difficulty);

      RefreshInfoPanel();
    }

    private void RefreshInfoPanel()
    {
      if (selectedLevelTitle != null)
        selectedLevelTitle.text = _selectedLevel != null
          ? (string.IsNullOrEmpty(_selectedLevel.DisplayName)
              ? $"Level {_selectedLevel.LevelNumber}"
              : _selectedLevel.DisplayName)
          : "—";

      if (selectedLevelSubtitle != null)
      {
        if (_selectedLevel == null) selectedLevelSubtitle.text = "";
        else
        {
          selectedLevelSubtitle.text =
            $"Starting credits: {_selectedLevel.StartingCredits * _selectedDifficulty.StartingCreditsMultiplier}    •    Base HP: {_selectedLevel.BaseHealth * _selectedDifficulty.BaseHealthMultiplier}";
        }
      }

      if (selectedDifficultyText != null)
        selectedDifficultyText.text = _selectedDifficulty != null
          ? $"Difficulty: <b>{_selectedDifficulty.DisplayName}</b>"
          : "Difficulty: —";

      if (selectedLevelIcon != null && _selectedLevel != null)
        selectedLevelIcon.sprite = _selectedLevel.LevelIcon;

      if (startButton != null)
        startButton.interactable =
          _selectedLevel != null &&
          _selectedDifficulty != null &&
          LevelProgress.IsUnlocked(_selectedLevel, levels);
    }

    private void StartGame()
    {
      if (_selectedLevel == null || _selectedDifficulty == null) return;
      if (!LevelProgress.IsUnlocked(_selectedLevel, levels)) return;

      if (string.IsNullOrEmpty(_selectedLevel.SceneName))
      {
        Debug.LogError($"[LevelSelect] У уровня '{_selectedLevel.name}' не задан SceneName.");
        return;
      }

      GameSession.Select(_selectedLevel, _selectedDifficulty);
      SceneManager.LoadScene(_selectedLevel.SceneName);
    }

    private void GoBack()
    {
      if (!string.IsNullOrEmpty(backSceneName))
        SceneManager.LoadScene(backSceneName);
    }
  }
}