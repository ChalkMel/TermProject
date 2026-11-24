using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoundSystem
{
    [CreateAssetMenu(fileName = "NewSoundLibrary", menuName = "Sound/Sound Library")]
    public class SoundLibrary : ScriptableObject
    {
        [field: SerializeField] private List<SoundConfig> sounds = new();

        private Dictionary<SoundType, SoundConfig> _map;

        public SoundConfig Get(SoundType type)
        {
            EnsureMap();
            return _map.TryGetValue(type, out var config) ? config : null;
        }

        private void EnsureMap()
        {
            _map = new Dictionary<SoundType, SoundConfig>(sounds.Count);
            foreach (var s in sounds)
            {
                _map[s.Type] = s;
            }
        }
    }
}