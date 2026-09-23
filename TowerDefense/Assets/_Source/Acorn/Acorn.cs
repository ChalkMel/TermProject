using _Source.Resources;
using UnityEngine;

namespace _Source.Acorn
{
  [RequireComponent(typeof(Collider2D))]
  public class Acorn : MonoBehaviour
  {
    private AcornSpawner _spawner;
    private Credits _credits;
    private int _reward;
    private bool _picked;

    public System.Action<Acorn> OnPickedUp;

    public void Initialize(AcornSpawner spawner, Credits credits, int reward)
    {
      _spawner = spawner;
      _credits = credits;
      _reward = reward;
    }

    private void OnMouseDown()
    {
      PickUp();
    }

    public void PickUp()
    {
      if (_picked) return;
      _picked = true;

      if (_credits != null) _credits.AddMoney(_reward);

      OnPickedUp?.Invoke(this);
      Destroy(gameObject);
    }
  }
}