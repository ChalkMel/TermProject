using UnityEngine;

namespace MiniGames.WolfAndEggs
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class WolfController : MonoBehaviour
    {
        [Header("Bounds")]
        [SerializeField] private float minX = -6f;
        [SerializeField] private float maxX = 6f;
        [SerializeField] private float moveSpeed = 8f;

        [Header("Input")]
        [SerializeField] private bool useKeyboard = true;
        [SerializeField] private bool usePointer = true;

        private bool _active = true;

        public bool IsActive => _active;

        private void Awake()
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            var col = GetComponent<Collider2D>();
            if (col == null)
                Debug.LogWarning("[WolfController] Нет Collider2D — волк не будет ловить предметы.");
            else
                col.isTrigger = true;
        }

        private void Update()
        {
            if (!_active) return;

            float targetX = transform.position.x;
            bool hasTarget = false;

            if (useKeyboard)
            {
                float axis = Input.GetAxisRaw("Horizontal");
                if (axis == 0f && Input.GetKey(KeyCode.A)) axis = -1f;
                if (axis == 0f && Input.GetKey(KeyCode.D)) axis = 1f;

                if (Mathf.Abs(axis) > 0.01f)
                {
                    targetX = transform.position.x + axis * moveSpeed * Time.deltaTime;
                    hasTarget = true;
                }
            }

            if (usePointer && (Input.GetMouseButton(0) || Input.touchCount > 0))
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float pointerX = Mathf.Clamp(wp.x, minX, maxX);
                targetX = Mathf.MoveTowards(transform.position.x, pointerX,
                                            moveSpeed * Time.deltaTime);
                hasTarget = true;
            }

            if (!hasTarget) return;

            Vector3 p = transform.position;
            p.x = Mathf.Clamp(targetX, minX, maxX);
            transform.position = p;
        }

        public void SetActive(bool value) => _active = value;
    }
}