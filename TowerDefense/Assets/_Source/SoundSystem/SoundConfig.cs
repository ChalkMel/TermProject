using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem
{
    [CreateAssetMenu(fileName = "NewSound", menuName = "Sound/Sound Config")]
    public class SoundConfig : ScriptableObject
    {
        [field: SerializeField] public SoundType Type { get; private set; }
        [field: SerializeField] public AudioClip Clip { get; private set; }
        [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }

        [Header("Playback")]
        [field: SerializeField, Range(0f, 1f)] public float Volume { get; private set; } = 1f;
        [field: SerializeField, Range(0f, 0.5f)] public float PitchVariation { get; private set; } = 0.05f;
        [field: SerializeField] public bool Loop { get; private set; }
    }
}