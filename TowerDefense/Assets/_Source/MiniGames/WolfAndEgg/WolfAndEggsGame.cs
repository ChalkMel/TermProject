using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGames.WolfAndEggs
{
    public class WolfAndEggsGame : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private EggSpawner spawner;
        [SerializeField] private WolfController wolf;

        [Header("Rules")]
        [SerializeField] private int startingScore = 0;
        [SerializeField] private int startingHealth = 3;
        [SerializeField] private int maxHealth = 5;
        [SerializeField] private bool scoreCanGoNegative = false;

        [Header("UI — In-Game")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("UI — Lose")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverText;

        [Header("UI — Win")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private TextMeshProUGUI winText;

        [Header("Restart")]
        [SerializeField] private string restartSceneName = "";

        private int _score;
        private int _health;
        private bool _ended;

        private void Awake()
        {
            _score = startingScore;
            _health = Mathf.Clamp(startingHealth, 0, maxHealth);
            _ended = false;

            if (spawner != null)
            {
                spawner.OnItemCaught       += HandleCaught;
                spawner.OnItemMissed       += HandleMissed;
                spawner.OnSequenceCompleted += HandleSequenceCompleted;
            }

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (winPanel != null)      winPanel.SetActive(false);
            UpdateUI();
        }

        private void Start()
        {
            spawner?.StartSpawning();
        }

        private void OnDestroy()
        {
            if (spawner == null) return;
            spawner.OnItemCaught       -= HandleCaught;
            spawner.OnItemMissed       -= HandleMissed;
            spawner.OnSequenceCompleted -= HandleSequenceCompleted;
        }

        private void Update()
        {
            if (_ended && Input.GetKeyDown(KeyCode.R)) Restart();
        }

        private void HandleCaught(FallingItem item)
        {
            if (_ended || item == null) return;
            var cfg = item.Config;
            if (cfg == null) return;

            Apply(cfg.CatchScore, cfg.CatchHealthDelta);
        }

        private void HandleMissed(FallingItem item)
        {
            if (_ended || item == null) return;
            var cfg = item.Config;
            if (cfg == null) return;

            Apply(cfg.MissScore, cfg.MissHealthDelta);
        }

        private void Apply(int scoreDelta, int healthDelta)
        {
            _score += scoreDelta;
            if (!scoreCanGoNegative) _score = Mathf.Max(0, _score);

            _health = Mathf.Clamp(_health + healthDelta, 0, maxHealth);
            UpdateUI();

            if (_health <= 0) Lose();
        }

        private void HandleSequenceCompleted()
        {
            if (_ended) return;
            if (_health > 0) Win();
        }

        private void UpdateUI()
        {
            if (scoreText != null)  scoreText.text  = $"Score: {_score}";
            if (healthText != null) healthText.text = $"HP: {_health}/{maxHealth}";
        }

        private void Lose()
        {
            if (_ended) return;
            _ended = true;

            wolf?.SetActive(false);
            spawner?.StopSpawning();

            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            if (gameOverText != null)
                gameOverText.text = $"Score: {_score}\nPress R to restart";
        }

        private void Win()
        {
            if (_ended) return;
            _ended = true;

            wolf?.SetActive(false);

            if (winPanel != null) winPanel.SetActive(true);
            if (winText != null)
                winText.text = $"You Win!\nScore: {_score}\nPress R to restart";
        }

        private void Restart()
        {
            string scene = string.IsNullOrEmpty(restartSceneName)
                ? SceneManager.GetActiveScene().name
                : restartSceneName;
            SceneManager.LoadScene(scene);
        }
    }
}