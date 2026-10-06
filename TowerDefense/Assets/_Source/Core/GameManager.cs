using EnemySystem;
using UnityEngine;
using BaseSystem;
using MenuSystems;

namespace Core
{
  public class GameManager : MonoBehaviour // убрать дебаг
  {
    [SerializeField] private Base baseRef;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private PauseMenu pauseMenu;

    private bool _ended;

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
      loseScreen.SetActive(true);
      EndGame();
    }

    private void HandleWin()
    {
      if (_ended) return;
      if (!baseRef.IsAlive) return;
      winScreen.SetActive(true);
      EndGame();
    }

    private void EndGame()
    {
      if (pauseMenu != null) pauseMenu.enabled = false;
      Time.timeScale = 0f;
    }
  }
}