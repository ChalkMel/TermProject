using EnemySystem;
using LevelSystem;
using UnityEngine;
using BaseSystem;
using Core.Core;
using MenuSystems;
using SoundSystem;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Base baseRef;
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject loseScreen;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private AudioService audioService;

        private bool _ended;
        [SerializeField] private GameFlowController gameFlow;

        private void Awake()
        {
            baseRef.OnBaseDestroyed += HandleLose;
            spawner.OnAllWavesCompleted += HandleWin;

            winScreen.SetActive(false);
            loseScreen.SetActive(false);
        }

        private void OnDestroy()
        {
            baseRef.OnBaseDestroyed -= HandleLose;
            spawner.OnAllWavesCompleted -= HandleWin;
        }

        private void HandleLose()
        {
            if (_ended) return;
            _ended = true;
            audioService?.StopMusic();
            audioService?.Play(SoundType.Lose);
            loseScreen.SetActive(true);
            gameFlow?.EndGame(false);
            EndGame();
        }

        private void HandleWin()
        {
            if (_ended) return;
            if (!baseRef.IsAlive) return;
            
            if (GameSession.CurrentLevel != null)
                LevelProgress.MarkCompleted(GameSession.CurrentLevel);

            audioService?.StopMusic();
            audioService?.Play(SoundType.Win); 
            winScreen.SetActive(true);
            gameFlow?.EndGame(true);
            EndGame();
        }

        private void EndGame()
        {
            if (pauseMenu != null) pauseMenu.enabled = false;
            Time.timeScale = 0f;
        }
    }
}