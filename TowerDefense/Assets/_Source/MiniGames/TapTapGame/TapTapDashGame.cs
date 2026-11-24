using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGames.TapTapDash
{
    public class TapTapDashGame : MonoBehaviour
    {
        [SerializeField] private TapTapDashRunner runner;

        [Header("UI — Score")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("UI — Lose")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        [Header("UI — Win")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private TextMeshProUGUI winScoreText;

        [Header("Restart")]
        [SerializeField] private string restartSceneName = "";

        private bool _ended;
        public event Action<bool> OnGameEnded;
        private bool _restartEnabled = true;

        private void Awake()
        {
            runner.OnScoreChanged += HandleScoreChanged;
            runner.OnFell         += HandleFell;
            runner.OnFinished     += HandleFinished;

            gameOverPanel.SetActive(false);
            winPanel.SetActive(false);
            UpdateScoreText(0);
        }

        private void OnDestroy()
        {
            runner.OnScoreChanged -= HandleScoreChanged;
            runner.OnFell         -= HandleFell;
            runner.OnFinished     -= HandleFinished;
        }

        private void HandleScoreChanged(int score) => UpdateScoreText(score);
        private void UpdateScoreText(int score)
        {
            if (scoreText != null) scoreText.text = $"Score: {score}";
        }

        private void HandleFell()
        {
            if (_ended) return;
            _ended = true;
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            if (finalScoreText != null && runner != null)
                finalScoreText.text = $"Score: {runner.Score}\n";
            OnGameEnded?.Invoke(false);
        }

        private void HandleFinished()
        {
            if (_ended) return;
            _ended = true;
            if (winPanel != null) winPanel.SetActive(true);
            if (winScoreText != null && runner != null)
                winScoreText.text = $"Level Complete!\nScore: {runner.Score}\n";
            OnGameEnded?.Invoke(true);
        }
    }
}