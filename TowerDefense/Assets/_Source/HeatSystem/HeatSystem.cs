using UnityEngine;
using BaseSystem;

namespace HeatSystem
{
    public class HeatSystem : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Base baseRef;
        [SerializeField] private AudioSource audioSource;

        [Header("Heat")]
        [SerializeField] private float maxHeat = 100f;
        [SerializeField] private float startHeat = 50f;
        [SerializeField] private float decayPerSecond = 2f;
        [SerializeField] private float heatPerPress = 8f;
        [SerializeField] private float pressCooldown = 0.15f;

        [Header("Overheat")]
        [SerializeField] private float overheatDamage = 5f;
        [SerializeField] private float overheatLockTime = 2f;

        private float _heat;
        private float _pressTimer;
        private float _lockTimer;
        private bool _isOverheated;

        public float Heat => _heat;
        public float MaxHeat => maxHeat;
        public float Normalized => maxHeat > 0f ? _heat / maxHeat : 0f;
        
        public bool IsOverheated => _isOverheated;
        
        public bool IsLocked => _lockTimer > 0f;

        public bool IsAlive => baseRef == null || baseRef.IsAlive;

        public System.Action<float, float> OnHeatChanged;
        public System.Action OnOverheated;
        public System.Action OnOverheatRecovered;

        private void Start()
        {
            _heat = Mathf.Clamp(startHeat, 0f, maxHeat);
            OnHeatChanged?.Invoke(_heat, maxHeat);
        }

        private void Update()
        {
            if (!IsAlive) return;

            if (_pressTimer > 0f) _pressTimer -= Time.deltaTime;

            if (_lockTimer > 0f)
            {
                _lockTimer -= Time.deltaTime;
                if (_lockTimer <= 0f)
                {
                    _lockTimer = 0f;
                    SetOverheated(false);
                }
            }

            if (_heat > 0f)
            {
                _heat -= decayPerSecond * Time.deltaTime;
                if (_heat < 0f) _heat = 0f;
                OnHeatChanged?.Invoke(_heat, maxHeat);
            }

            if (_heat <= 0f) Lose();
            if(_heat <=10 && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        public void Press()
        {
            if (!IsAlive) return;
            if (_lockTimer > 0f) return;
            if (_pressTimer > 0f) return;

            _pressTimer = pressCooldown;

            if (_heat >= maxHeat)
            {
                TriggerOverheat();
                return;
            }

            _heat = Mathf.Min(_heat + heatPerPress, maxHeat);
            OnHeatChanged?.Invoke(_heat, maxHeat);

            if (_heat >= maxHeat)
                TriggerOverheat();
        }

        private void TriggerOverheat()
        {
            if (baseRef != null) baseRef.GetDamage(overheatDamage);

            _heat = maxHeat;
            _lockTimer = overheatLockTime;
            OnHeatChanged?.Invoke(_heat, maxHeat);

            SetOverheated(true);
            Debug.Log($"[HeatSystem] Перегрев! Урон чайнику: {overheatDamage}");
        }

        private void SetOverheated(bool value)
        {
            if (_isOverheated == value) return;
            _isOverheated = value;

            if (value) OnOverheated?.Invoke();
            else       OnOverheatRecovered?.Invoke();
        }

        private void Lose()
        {
            if (baseRef != null && baseRef.IsAlive)
                baseRef.GetDamage(baseRef.MaxHealth * 2f);
            SetOverheated(false);
            Debug.Log("[HeatSystem] Тепло кончилось — проигрыш.");
            enabled = false;
        }

        public void SetMaxHeat(float value) => maxHeat = Mathf.Max(1f, value);
        public void SetDecay(float perSecond) => decayPerSecond = perSecond;
    }
}