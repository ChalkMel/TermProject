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

        public void Build(IList<DifficultyConfig> difficulties)
        {
            Clear();
            if (buttonPrefab == null || container == null) return;

            foreach (var cfg in difficulties)
            {
                if (cfg == null) continue;

                var btn = Instantiate(buttonPrefab, container);
                btn.Bind(cfg);
                btn.OnClick += b => DifficultyClicked?.Invoke(b.Config);
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