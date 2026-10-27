using System;
using UnityEngine;

namespace MiniGames.Balance
{
    public class TeapotOnPlatform : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Transform player;

        [Header("Platform")]
        [SerializeField] private Vector2 halfExtents = new Vector2(0.7f, 0.5f);
        [SerializeField] private Vector2 baseOffset = new Vector2(0f, 0.7f);

        [Header("Spring")]
        [SerializeField] private float springStrength = 12f;
        [SerializeField] private float damping = 6f;

        [Header("Visual — Base")]
        [SerializeField] private float tiltDegreesPerUnit = 20f;

        [Header("Visual — Color Warning")]
        [Tooltip("SpriteRenderer чайника. Если пусто — ищется на этом или дочерних объектах.")]
        [SerializeField] private SpriteRenderer teapotRenderer;
        
        [SerializeField] private Color cautionColor = new Color(1.00f, 0.85f, 0.20f);
        [SerializeField] private Color dangerColor  = new Color(1.00f, 0.40f, 0.15f);
        [SerializeField] private Color criticalColor = new Color(1.00f, 0.15f, 0.15f);

        [Tooltip("Порог (нормированный 0..1), выше которого начинается опасная зона и пульсация.")]
        [SerializeField] private float criticalThreshold = 0.75f;

        [Tooltip("Амплитуда пульсации цвета в критической зоне (0..1). 0 — без пульсации.")]
        [SerializeField] private float pulseAmplitude = 0.25f;
        [SerializeField] private float pulseSpeed = 10f;

        [Tooltip("Амплитуда тряски чайника в критической зоне (мировые единицы).")]
        [SerializeField] private float criticalShakeAmount = 0.03f;
        [SerializeField] private float criticalShakeSpeed = 30f;

        [Header("Fall")]
        [SerializeField] private float fallGravity = 20f;
        [SerializeField] private float fallRotateSpeed = 360f;
        [SerializeField] private float fallYThreshold = -15f;

        private Vector2 _worldPos;
        private Vector2 _velocity;
        private bool _hasFallen;
        private bool _isDead;
        private float _fallVy;
        private Color _baseTeapotColor = Color.white;
        private Transform _teapotVisual;

        public bool HasFallen => _hasFallen;
        public bool IsDead => _isDead;
        public Vector2 Offset => _worldPos - TargetPoint;
        public float NormalizedDanger { get; private set; }

        public event Action OnFallen;

        private Vector2 TargetPoint => (Vector2)player.position + baseOffset;

        private void Start()
        {
            if (player == null)
            {
                Debug.LogError("[TeapotOnPlatform] Не назначен player.");
                enabled = false;
                return;
            }
            
            if (teapotRenderer == null)
                teapotRenderer = GetComponentInChildren<SpriteRenderer>();

            if (teapotRenderer != null)
            {
                _baseTeapotColor = teapotRenderer.color;
                _teapotVisual = teapotRenderer.transform;
            }

            _worldPos = TargetPoint;
            ApplyTransform();
            ApplyColor(0f);
        }

        private void Update()
        {
            if (_isDead) return;
            if (_hasFallen) { UpdateFalling(); return; }

            Vector2 target = TargetPoint;
            Vector2 toTarget = target - _worldPos;
            _velocity += toTarget * springStrength * Time.deltaTime;
            _velocity *= Mathf.Exp(-damping * Time.deltaTime);
            _worldPos += _velocity * Time.deltaTime;

            Vector2 offset = _worldPos - target;
            
            float nx = Mathf.Abs(offset.x) / Mathf.Max(0.0001f, halfExtents.x);
            float ny = Mathf.Abs(offset.y) / Mathf.Max(0.0001f, halfExtents.y);
            NormalizedDanger = Mathf.Max(nx, ny);

            if (NormalizedDanger >= 1f)
            {
                StartFalling();
                return;
            }

            ApplyTransform();
            ApplyColor(NormalizedDanger);
        }

        private void ApplyTransform()
        {
            Vector3 pos = new Vector3(_worldPos.x, _worldPos.y, 0f);
            
            if (NormalizedDanger > criticalThreshold && _teapotVisual != null)
            {
                float shakeK = Mathf.InverseLerp(criticalThreshold, 1f, NormalizedDanger);
                float t = Time.time * criticalShakeSpeed;
                pos.x += (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f * criticalShakeAmount * shakeK;
                pos.y += (Mathf.PerlinNoise(0f, t) - 0.5f) * 2f * criticalShakeAmount * shakeK;
            }

            transform.position = pos;

            Vector2 target = TargetPoint;
            float tiltZ = -(pos.x - target.x) * tiltDegreesPerUnit;
            transform.rotation = Quaternion.Euler(0f, 0f, tiltZ);
        }

        private void ApplyColor(float danger01)
        {
            if (teapotRenderer == null) return;
            
            Color target;
            if (danger01 <= 0.5f)
            {
                float k = Mathf.InverseLerp(0f, 0.5f, danger01);
                target = Color.Lerp(Color.white, cautionColor, k);
            }
            else if (danger01 <= criticalThreshold)
            {
                float k = Mathf.InverseLerp(0.5f, criticalThreshold, danger01);
                target = Color.Lerp(cautionColor, dangerColor, k);
            }
            else
            {
                float k = Mathf.InverseLerp(criticalThreshold, 1f, danger01);
                target = Color.Lerp(dangerColor, criticalColor, k);
            }
            
            if (danger01 > criticalThreshold && pulseAmplitude > 0f)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
                float strength = Mathf.InverseLerp(criticalThreshold, 1f, danger01);
                target = Color.Lerp(target, Color.white,
                                    pulse * pulseAmplitude * strength);
            }
            
            teapotRenderer.color = new Color(
                target.r * _baseTeapotColor.r,
                target.g * _baseTeapotColor.g,
                target.b * _baseTeapotColor.b,
                _baseTeapotColor.a);
        }

        private void StartFalling()
        {
            if (_hasFallen) return;
            _hasFallen = true;
            _fallVy = 0f;
            if (teapotRenderer != null) teapotRenderer.color = criticalColor;
            OnFallen?.Invoke();
        }

        private void UpdateFalling()
        {
            _fallVy -= fallGravity * Time.deltaTime;
            Vector3 p = transform.position;
            p.y += _fallVy * Time.deltaTime;
            p.x += _velocity.x * Time.deltaTime;
            transform.position = p;
            transform.Rotate(0f, 0f, -fallRotateSpeed * Time.deltaTime);

            if (transform.position.y < fallYThreshold)
                _isDead = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (player == null) return;
            Vector2 target = (Vector2)player.position + baseOffset;
            Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.8f);
            Gizmos.DrawWireCube(new Vector3(target.x, target.y, 0f),
                                new Vector3(halfExtents.x * 2f, halfExtents.y * 2f, 0.1f));
        }
    }
}