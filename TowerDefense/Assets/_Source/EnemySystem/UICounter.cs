using TMPro;
using UnityEngine;

namespace _Source.EnemySystem
{
  public class WaveCounterUI : MonoBehaviour
  {
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private TextMeshProUGUI counterText;

    private void Awake()
    {
      spawner.OnCountersChanged += Draw;
      Draw(0, 0);
    }

    private void OnDestroy()
    {
      spawner.OnCountersChanged -= Draw;
    }

    private void Draw(int killed, int total)
    {
      counterText.text = $"Killed: {killed} / {total}";
    }
  }
}