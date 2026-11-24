using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelSystem.UI
{
    public class LevelCardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Color normalColor   = new(0.15f, 0.18f, 0.22f, 1f);
        [SerializeField] private Color hoverColor    = new(0.22f, 0.27f, 0.32f, 1f);
        [SerializeField] private Color selectedColor = new(0.30f, 0.55f, 0.35f, 1f);
        [SerializeField] private Color lockedColor   = new(0.08f, 0.08f, 0.10f, 1f);
        [SerializeField] private GameObject lockBadge;
        [SerializeField] private GameObject completedBadge;

        private Button _button;
        private bool _selected;
        private bool _locked;
        private LevelConfig _config;

        public LevelConfig Config => _config;
        public bool IsLocked => _locked;
        public event Action<LevelConfig> Clicked;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleClick);
        }

        public void Bind(LevelCardViewModel vm)
        {
            _config = vm.Config;
            _locked = vm.Locked;

            if (titleText != null)    titleText.text    = vm.Title;
            if (subtitleText != null) subtitleText.text = vm.Subtitle;

            if (icon != null)
            {
                icon.sprite  = vm.Icon;
                icon.enabled = true;
            }

            if (lockBadge != null)      lockBadge.SetActive(vm.Locked);
            if (completedBadge != null) completedBadge.SetActive(!vm.Locked && vm.Completed);

            _button.interactable = !vm.Locked;

            if (background != null)
                background.color = vm.Locked ? lockedColor : normalColor;

            _selected = false;
        }

        public void SetSelected(bool selected)
        {
            if (_locked) return;
            _selected = selected;
            if (background != null)
                background.color = selected ? selectedColor : normalColor;
        }

        public void OnPointerEnter(PointerEventData e)
        {
            if (_locked) return;
            if (!_selected && background != null) background.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData e)
        {
            if (_locked) return;
            if (!_selected && background != null) background.color = normalColor;
        }

        private void HandleClick()
        {
            if (_locked) return;
            Clicked?.Invoke(_config);
        }
    }
}