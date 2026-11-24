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

        public void Build(IList<LevelCardViewModel> items)
        {
            Clear();
            if (cardPrefab == null || container == null) return;

            foreach (var vm in items)
            {
                if (vm.Config == null) continue;

                var card = Instantiate(cardPrefab, container);
                card.Bind(vm);
                card.Clicked += cfg => LevelClicked?.Invoke(cfg);
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