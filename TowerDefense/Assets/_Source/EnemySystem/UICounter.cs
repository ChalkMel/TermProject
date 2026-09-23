using TMPro;
using UnityEngine;
using _Source.EnemySystem;

namespace _Source.UI
{
  public class WaveCounterUI : MonoBehaviour
  {
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private TextMeshProUGUI waveIndexText;
    [SerializeField] private TextMeshProUGUI counterText;

    private void Awake()
    {
      if (spawner == null) return;

      spawner.OnWaveStarted += HandleWaveStarted;
      spawner.OnWaveCountersChanged += Draw;

      if (waveIndexText != null) waveIndexText.text = "";
      if (counterText != null) counterText.text = "0 / 0";
    }

    private void OnDestroy()
    {
      if (spawner == null) return;

      spawner.OnWaveStarted -= HandleWaveStarted;
      spawner.OnWaveCountersChanged -= Draw;
    }

    private void HandleWaveStarted(int waveIndex)
    {
      if (waveIndexText != null)
        waveIndexText.text = $"Wave {waveIndex + 1}";
    }

    private void Draw(int killed, int total, int waveIndex)
    {
      if (counterText != null)
        counterText.text = $"Wave {waveIndex}:{killed} / {total}";
    }
  }
}