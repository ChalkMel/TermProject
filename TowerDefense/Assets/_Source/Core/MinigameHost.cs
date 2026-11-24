using System;
using System.Collections;
using System.Collections.Generic;
using Core.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class MinigameHost : MonoBehaviour
    {
        public event Action<bool> Finished;

        private Scene _scene;
        private bool _running;

        private Camera _mainCamera;
        private AudioListener _mainListener;
        private readonly List<Canvas> _mainCanvases = new();

        public void Launch(MinigameConfig config)
        {
            if (_running) return;
            StartCoroutine(LaunchRoutine(config.SceneName));
        }

        private IEnumerator LaunchRoutine(string sceneName)
        {
            _running = true;
            CacheMainSceneVisuals();
            SetMainSceneVisuals(false);

            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _scene = SceneManager.GetSceneByName(sceneName);

            yield return null;
            SubscribeToMinigame();

            while (_running) yield return null;
        }

        private void SubscribeToMinigame()
        {
            foreach (var root in _scene.GetRootGameObjects())
            {
                if (root.TryGetComponent<MiniGames.TapTapDash.TapTapDashGame>(out var ttd))
                {
                    ttd.OnGameEnded += HandleResult;
                    return;
                }
                if (root.TryGetComponent<MiniGames.WolfAndEggs.WolfAndEggsGame>(out var wae))
                {
                    wae.OnGameEnded += HandleResult;
                    return;
                }
            }
            HandleResult(false);
        }

        private void HandleResult(bool won)
        {
            if (!_running) return;
            _running = false;
            StartCoroutine(UnloadRoutine(won));
        }

        private IEnumerator UnloadRoutine(bool won)
        {
            UnsubscribeFromMinigame();
            if (_scene.IsValid() && _scene.isLoaded)
                yield return SceneManager.UnloadSceneAsync(_scene);
            _scene = default;

            Time.timeScale = 1f;
            SetMainSceneVisuals(true);
            Finished?.Invoke(won);
        }

        private void UnsubscribeFromMinigame()
        {
            if (!_scene.IsValid()) return;
            foreach (var root in _scene.GetRootGameObjects())
            {
                if (root.TryGetComponent<MiniGames.TapTapDash.TapTapDashGame>(out var ttd))
                    ttd.OnGameEnded -= HandleResult;
                if (root.TryGetComponent<MiniGames.WolfAndEggs.WolfAndEggsGame>(out var wae))
                    wae.OnGameEnded -= HandleResult;
            }
        }

        private void CacheMainSceneVisuals()
        {
            _mainCamera = Camera.main;
            _mainCanvases.Clear();

            var active = SceneManager.GetActiveScene();
            foreach (var root in active.GetRootGameObjects())
            {
                if (_mainListener == null)
                    _mainListener = root.GetComponentInChildren<AudioListener>();
                _mainCanvases.AddRange(root.GetComponentsInChildren<Canvas>(includeInactive: false));
            }
        }

        private void SetMainSceneVisuals(bool visible)
        {
            if (_mainCamera  != null) _mainCamera.enabled  = visible;
            if (_mainListener != null) _mainListener.enabled = visible;
            foreach (var c in _mainCanvases)
                if (c != null) c.enabled = visible;
        }
    }
}