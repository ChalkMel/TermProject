using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelSystem.UI
{
    public class LevelListView : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private LevelCardButton cardPrefab;

        private readonly List<LevelCardButton> _cards = new();

        public event Action<LevelConfig> LevelClicked;

        public void Build(IList<LevelConfig> levels)
        {
            Clear();
            if (cardPrefab == null || container == null) return;

            foreach (var cfg in levels)
            {
                if (cfg == null) continue;

                bool locked    = !LevelProgress.IsUnlocked(cfg, levels);
                bool completed =  LevelProgress.IsCompleted(cfg);

                var card = Instantiate(cardPrefab, container);
                card.Bind(cfg, locked, completed);
                card.OnClick += c => LevelClicked?.Invoke(c.Config);
                _cards.Add(card);
            }
        }

        public void SetSelected(LevelConfig level)
        {
            foreach (var c in _cards)
                c.SetSelected(c.Config == level);
        }

        private void Clear()
        {
            foreach (var c in _cards)
                if (c != null) Destroy(c.gameObject);
            _cards.Clear();
        }
    }
}