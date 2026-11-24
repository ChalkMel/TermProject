using System;
using Resources;
using Pool;
using SoundSystem;
using UnityEngine;

namespace AcornSystem
{
  [RequireComponent(typeof(Collider2D))]
  public class Acorn : MonoBehaviour, IPoolableItem
  {
    private AcornSpawner _spawner;
    private Credits _credits;
    private int _reward;
    private bool _picked;
    private Collider2D _collider;
    private Action _returnCallback;
    private IAudioService _audio;

    public event Action<Acorn> OnPickedUp;

    private void Awake()
    {
      _collider = GetComponent<Collider2D>();
    }

    public void Initialize(AcornSpawner spawner, Credits credits, int reward, IAudioService audio)
    {
      _spawner = spawner;
      _credits = credits;
      _reward = reward;
      _audio = audio;
    }

    public void SetCallback(Action callback) => _returnCallback = callback;

    public void Activate()
    {
      _picked = false;
      if (_collider != null) _collider.enabled = true;
      gameObject.SetActive(true);
    }

    public void Deactivate()
    {
      if (_collider != null) _collider.enabled = false;
      gameObject.SetActive(false);
    }

    private void OnMouseDown() => PickUp();

    public void PickUp()
    {
      if (_picked) return;
      _picked = true;

      _audio?.Play(SoundType.AcornPickup);
      if (_credits != null) _credits.AddMoney(_reward);
      OnPickedUp?.Invoke(this);
      _returnCallback?.Invoke();
    }
  }
}