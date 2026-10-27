using UnityEngine;

namespace MiniGames.TapTapDash
{
  public class TapTapDashFinish : MonoBehaviour
  {
    [Tooltip("Радиус попадания. Чуть больше половины клетки — прощает мелкие промахи.")]
    public float Radius { get; private set; }

    private void OnDrawGizmos()
    {
      Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.9f);
      Gizmos.DrawWireSphere(transform.position, Radius);
    }
  }
}