using System;
using System.Collections;
using Pool;
using UnityEngine;

namespace SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : MonoBehaviour, IPoolableItem
    {
        private AudioSource _source;
        private Action _returnCallback;
        private Coroutine _returnRoutine;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void SetCallback(Action callback) => _returnCallback = callback;

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate()
        {
            if (_returnRoutine != null)
            {
                StopCoroutine(_returnRoutine);
                _returnRoutine = null;
            }
            if (_source != null) _source.Stop();
            gameObject.SetActive(false);
        }

        public void Play(SoundConfig config, Vector3? position = null)
        {
            if (config == null || config.Clip == null) return;

            _source.clip                  = config.Clip;
            _source.outputAudioMixerGroup = config.MixerGroup;
            _source.volume                = config.Volume;
            _source.pitch                 = 1f + UnityEngine.Random.Range(
                                                -config.PitchVariation,
                                                 config.PitchVariation);
            _source.loop                  = config.Loop;
            

            _source.Play();

            if (!config.Loop)
                _returnRoutine = StartCoroutine(
                    ReturnAfter(config.Clip.length / Mathf.Abs(_source.pitch)));
        }

        private IEnumerator ReturnAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            _returnRoutine = null;
            ReturnToPool();
        }

        public void ReturnToPool()
        {
            _returnRoutine = null;
            _returnCallback?.Invoke();
        }
    }
}