using UnityEngine;

namespace MiniGames.WolfAndEggs
{
    [RequireComponent(typeof(Collider2D))]
    public class FallingItem : MonoBehaviour
    {
        [Tooltip("Ниже этого Y предмет считается упавшим мимо.")]
        [SerializeField] private float missYThreshold = -8f;

        private FallingItemConfig _config;
        private SpriteRenderer _sr;
        private bool _consumed;

        public FallingItemConfig Config => _config;

        public System.Action<FallingItem> OnCaught;
        public System.Action<FallingItem> OnMissed;

        private void Awake()
        {
            _sr = GetComponentInChildren<SpriteRenderer>();

            var col = GetComponent<Collider2D>();
            col.isTrigger = true;

            var rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0f;
            }
        }

        public void Initialize(FallingItemConfig config)
        {
            _config = config;
            _consumed = false;

            if (_sr != null)
            {
                if (config.Sprite != null) _sr.sprite = config.Sprite;
                _sr.color = config.Tint;
            }
        }

        private void Update()
        {
            if (_consumed || _config == null) return;

            transform.position += Vector3.down * (_config.FallSpeed * Time.deltaTime);

            if (transform.position.y < missYThreshold)
                MarkMissed();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_consumed) return;
            if (other.GetComponentInParent<WolfController>() == null) return;
            MarkCaught();
        }

        private void MarkCaught()
        {
            if (_consumed) return;
            _consumed = true;
            OnCaught?.Invoke(this);
            Destroy(gameObject);
        }

        private void MarkMissed()
        {
            if (_consumed) return;
            _consumed = true;
            OnMissed?.Invoke(this);
            Destroy(gameObject);
        }
    }
}