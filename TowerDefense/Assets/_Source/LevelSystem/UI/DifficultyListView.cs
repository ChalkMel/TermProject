using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSystem.UI
{
    public class DifficultyListView : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private DifficultyButton buttonPrefab;

        private readonly List<DifficultyButton> _buttons = new();

        public event Action<DifficultyConfig> DifficultyClicked;

        public void Build(IList<DifficultyViewModel> items)
        {
            Clear();
            if (buttonPrefab == null || container == null) return;

            foreach (var vm in items)
            {
                if (vm.Config == null) continue;
                var btn = Instantiate(buttonPrefab, container);
                btn.Bind(vm);
                btn.Clicked += cfg => DifficultyClicked?.Invoke(cfg);
                _buttons.Add(btn);
            }
        }

        public void SetSelected(DifficultyConfig difficulty)
        {
            foreach (var b in _buttons)
                b.SetSelected(b.Config == difficulty);
        }

        private void Clear()
        {
            foreach (var b in _buttons)
                if (b != null) Destroy(b.gameObject);
            _buttons.Clear();
        }
    }
}