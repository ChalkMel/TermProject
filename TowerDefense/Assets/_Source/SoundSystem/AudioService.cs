using UnityEngine;

namespace SoundSystem
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        [Header("Library")]
        [SerializeField] private SoundLibrary library;

        [Header("Pool")]
        [SerializeField] private PooledAudioSource audioSourcePrefab;
        [SerializeField] private int maxPoolSize  = 16;
        [SerializeField] private int startPoolSize = 4;

        [Header("Music")]
        [SerializeField] private AudioSource musicSource;

        private ObjectPool<PooledAudioSource> _pool;
        private SoundType? _currentMusic;

        public static AudioService Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (audioSourcePrefab == null)
            {
                Debug.LogError("[AudioService] audioSourcePrefab is not assigned!");
                enabled = false;
                return;
            }

            var factory = new Factory<PooledAudioSource>(audioSourcePrefab);
            _pool = new ObjectPool<PooledAudioSource>(factory, maxPoolSize, startPoolSize);

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop        = true;
                musicSource.playOnAwake = false;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Play(SoundType type)
        {
            var config = library != null ? library.Get(type) : null;
            if (config == null) return;

            if (_pool.TryGet(out var source))
                source.Play(config);
        }

        public void PlayAt(SoundType type, Vector3 position)
        {
            var config = library != null ? library.Get(type) : null;
            if (config == null) return;

            if (_pool.TryGet(out var source))
                source.Play(config, position);
        }

        public void PlayMusic(SoundType type)
        {
            if (_currentMusic == type) return;

            var config = library != null ? library.Get(type) : null;
            if (config == null || config.Clip == null) return;

            _currentMusic                     = type;
            musicSource.clip                  = config.Clip;
            musicSource.outputAudioMixerGroup = config.MixerGroup;
            musicSource.volume                = config.Volume;
            musicSource.loop                  = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            _currentMusic = null;
            musicSource.Stop();
        }
    }
}