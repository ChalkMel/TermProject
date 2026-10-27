using UnityEngine;

namespace MiniGames.Balance
{
  public class BalanceWalker : MonoBehaviour
  {
    [Header("Path")]
    [SerializeField] private BalancePath path;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 10f;

    private Vector2 _velocity;

    public Vector2 CurrentVelocity => _velocity;
    public bool IsActive { get; set; } = true;

    private void Update()
    {
      if (!IsActive)
      {
        _velocity = Vector2.zero;
        return;
      }

      Vector2 input = new Vector2(
        Input.GetAxisRaw("Horizontal"),
        Input.GetAxisRaw("Vertical"));
      if (input.sqrMagnitude > 1f) input.Normalize();

      Vector2 targetVel = input * maxSpeed;
      float rate = input.sqrMagnitude > 0.01f ? acceleration : deceleration;
      _velocity = Vector2.MoveTowards(_velocity, targetVel, rate * Time.deltaTime);

      Vector2 current = transform.position;
      Vector2 desired = current + _velocity * Time.deltaTime;

      if (CanMoveTo(desired))
      {
        transform.position = desired;
      }
      else if (CanMoveTo(new Vector2(desired.x, current.y)))
      {
        transform.position = new Vector2(desired.x, current.y);
        _velocity.y = 0f;
      }
      else if (CanMoveTo(new Vector2(current.x, desired.y)))
      {
        transform.position = new Vector2(current.x, desired.y);
        _velocity.x = 0f;
      }
      else
      {
        _velocity = Vector2.zero;
      }
    }

    private bool CanMoveTo(Vector2 pos) => path == null || path.IsOnPath(pos);
  }
}