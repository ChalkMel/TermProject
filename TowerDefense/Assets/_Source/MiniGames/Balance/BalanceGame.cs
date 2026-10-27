using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGames.Balance
{
    public class BalanceGame : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private BalanceWalker walker;
        [SerializeField] private TeapotOnPlatform teapot;

        [Header("Finish")]
        [Tooltip("Маркер в конце дорожки.")]
        [SerializeField] private Transform finish;
        [SerializeField] private float finishRadius = 0.6f;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private TextMeshProUGUI winText;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private TextMeshProUGUI loseText;

        [Header("Restart")]
        [SerializeField] private string restartSceneName = "";

        private bool _ended;

        private void Awake()
        {
            if (teapot != null) teapot.OnFallen += HandleFall;
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (teapot != null) teapot.OnFallen -= HandleFall;
        }

        private void Update()
        {
            if (_ended)
            {
                if (Input.GetKeyDown(KeyCode.R)) Restart();
                return;
            }

            if (walker != null && finish != null)
            {
                float d = Vector2.Distance(walker.transform.position, finish.position);
                if (d <= finishRadius) Win();
            }
        }

        private void HandleFall() => Lose();

        private void Win()
        {
            if (_ended) return;
            _ended = true;
            if (walker != null) walker.IsActive = false;
            if (winPanel != null) winPanel.SetActive(true);
            if (winText != null) winText.text = "You Win!\nPress R to restart";
        }

        private void Lose()
        {
            if (_ended) return;
            _ended = true;
            if (walker != null) walker.IsActive = false;
            if (losePanel != null) losePanel.SetActive(true);
            if (loseText != null) loseText.text = "The teapot fell!\nPress R to restart";
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