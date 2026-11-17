using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public class AbilityBar : MonoBehaviour
    {
        [SerializeField] private AbilityController controller;
        [SerializeField] private AbilityButton buttonPrefab;

        private readonly List<AbilityButton> _spawned = new();

        public void Init(AbilityController ctrl, IReadOnlyList<AbilityConfig> levelAbilities)
        {
            controller = ctrl;

            foreach (var b in _spawned)
                if (b != null) Destroy(b.gameObject);
            _spawned.Clear();

            if (controller == null || levelAbilities == null || buttonPrefab == null) return;

            foreach (var cfg in levelAbilities)
            {
                if (cfg == null) continue;
                var btn = Instantiate(buttonPrefab, transform);
                btn.Init(controller, cfg);
                _spawned.Add(btn);
            }
        }
    }
}