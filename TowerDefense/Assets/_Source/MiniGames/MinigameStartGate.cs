using System.Collections;
using UnityEngine;

namespace MiniGames
{
    public class MinigameStartGate : MonoBehaviour
    {
        [Tooltip("Оверлей с надписью Tap to start / кнопкой Start. Скрывается после старта.")]
        [SerializeField] private GameObject overlay;

        private bool _started;

        private void Awake()
        {
            Time.timeScale = 0f;
            if (overlay != null) overlay.SetActive(true);
        }

        private void OnDestroy()
        {
            if (!_started) Time.timeScale = 1f;
        }

        private void Update()
        {
            if (_started) return;
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                StartGame();
        }

        private void StartGame()
        {
            _started = true;
            if (overlay != null) overlay.SetActive(false);
            StartCoroutine(ReleasePauseNextFrame());
        }

        private IEnumerator ReleasePauseNextFrame()
        {
            yield return null;
            Time.timeScale = 1f;
        }
    }
}