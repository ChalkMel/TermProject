using UnityEngine;

namespace MiniGames.TapTapDash
{
  public class TapTapDashFinish : MonoBehaviour
  {
    [Tooltip("Радиус попадания. Чуть больше половины клетки — прощает мелкие промахи.")]
    [SerializeField] private float radius = 0.6f;

    public float Radius => radius;

    private void OnDrawGizmos()
    {
      Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.9f);
      Gizmos.DrawWireSphere(transform.position, radius);
    }
  }
}