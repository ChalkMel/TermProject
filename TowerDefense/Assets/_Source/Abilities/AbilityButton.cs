using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Abilities
{
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image cooldownOverlay;

        private AbilityController _controller;
        private AbilityConfig _config;

        public AbilityType Type => _config != null ? _config.Type : default;

        public void Init(AbilityController controller, AbilityConfig config)
        {
            _controller = controller;
            _config = config;

            if (button != null) button.onClick.AddListener(HandleClick);
            if (label != null) label.text = config.DisplayName;
        }

        private void OnDestroy()
        {
            if (button != null) button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick() => _controller?.TryActivate(_config.Type);

        private void Update()
        {
            if (_controller == null || _config == null) return;
            
            bool canUse   = _controller.CanActivate(_config.Type);

            if (button != null) button.interactable = canUse;

            if (cooldownOverlay != null)
            {
                float remaining = _controller.CooldownRemaining(_config.Type);
                cooldownOverlay.fillAmount = _config.Cooldown > 0f ? remaining / _config.Cooldown : 0f;
            }

            if (label != null)
            {
                label.text = _controller.IsOnCooldown(_config.Type) ? $"{_config.DisplayName} ({_controller.CooldownRemaining(_config.Type):0.0}s)" : $"{_config.DisplayName} ({_config.ManaCost})";
            }
        }
    }
}