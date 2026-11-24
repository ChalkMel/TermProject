using UnityEngine;

namespace SoundSystem
{
    public interface IAudioService
    {
        void Play(SoundType type);
        void PlayAt(SoundType type, Vector3 position);
        void PlayMusic(SoundType type);
        void StopMusic();
    }
}