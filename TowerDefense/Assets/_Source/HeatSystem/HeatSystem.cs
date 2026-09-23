using UnityEngine;
using _Source;

namespace _Source.HeatSystem
{
    public class HeatSystem : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Base baseRef;

        [Header("Heat")]
        [SerializeField] private float maxHeat = 100f;
        [SerializeField] private float startHeat = 50f;
        [SerializeField] private float decayPerSecond = 2f;
        [SerializeField] private float heatPerPress = 8f;
        [SerializeField] private float pressCooldown = 0.15f;

        [Header("Overheat")]
        [SerializeField] private float overheatDamage = 5f;
        [SerializeField] private float overheatLockTime = 2f;
        [SerializeField] private float overheatSpikePerPress = 3f;

        private float _heat;
        private float _pressTimer;
        private float _lockTimer;

        public float Heat => _heat;
        public float MaxHeat => maxHeat;
        public float Normalized => maxHeat > 0f ? _heat / maxHeat : 0f;
        public bool IsOverheated => _lockTimer > 0f;
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
                if (_lockTimer <= 0f) OnOverheatRecovered?.Invoke();
            }
            
            if((_heat is > 0 and <= 20) || _heat> 90)
            {
                OnOverheated?.Invoke();
            }
            else if (_heat is > 0 and > 20 and < 90 )
            {
                OnOverheatRecovered?.Invoke();
            }
            if (_heat > 0f)
            {
                _heat -= decayPerSecond * Time.deltaTime;
                if (_heat < 0f) _heat = 0f;
                OnHeatChanged?.Invoke(_heat, maxHeat);
            }

            if (_heat <= 0f) Lose();
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

            _lockTimer = overheatLockTime;
            _heat = maxHeat;
            OnHeatChanged?.Invoke(_heat, maxHeat);
            OnOverheated?.Invoke();

            Debug.Log($"[HeatSystem] Перегрев! Урон чайнику: {overheatDamage}");
        }

        private void Lose()
        {
            if (baseRef != null && baseRef.IsAlive)
                baseRef.GetDamage(baseRef.MaxHealth * 2f);

            Debug.Log("[HeatSystem] Тепло кончилось — проигрыш.");
            enabled = false;
        }
        
        public void SetMaxHeat(float value) => maxHeat = Mathf.Max(1f, value);
        public void SetDecay(float perSecond) => decayPerSecond = perSecond;
    }
}