using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.WolfAndEggs
{
    [Serializable]
    public class SpawnEntry
    {
        public FallingItemConfig Config;
        [Tooltip("Позиция X в мире, откуда упадёт предмет.")]
        public float XPosition = 0f;
        [Tooltip("Пауза перед спавном (от предыдущего предмета).")]
        public float DelayBefore = 0.5f;
    }

    public class EggSpawner : MonoBehaviour
    {
        [Header("Level Sequence")]
        [SerializeField] private List<SpawnEntry> sequence = new();
        [SerializeField] private bool loopSequence = false;
        [SerializeField] private float loopPause = 2f;

        [Header("Spawn Point")]
        [SerializeField] private float spawnY = 7f;
        [SerializeField] private Transform parent;
        [Tooltip("Рамки спавна по x")]
        [SerializeField] private Vector2 range;

        private Coroutine _routine;
        private bool _running;
        private int _activeItems;
        
        public event Action<FallingItem> OnItemCaught;
        public event Action<FallingItem> OnItemMissed;
        public event Action OnSequenceCompleted;

        private void Awake()
        {
            if (parent == null) parent = transform;
        }

        public void StartSpawning()
        {
            if (_running) return;
            if (sequence.Count == 0)
            {
                return;
            }
            _running = true;
            _activeItems = 0;
            _routine = StartCoroutine(SpawnRoutine());
        }

        public void StopSpawning()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
            _running = false;
        }

        private IEnumerator SpawnRoutine()
        {
            do
            {
                foreach (var entry in sequence)
                {
                    if (entry.Config == null) continue;
                    yield return new WaitForSeconds(entry.DelayBefore);
                    Spawn(entry);
                }

                if (!loopSequence) break;
                yield return new WaitForSeconds(loopPause);
            } while (loopSequence);

            _running = false;
            _routine = null;
            TryComplete();
        }

        private void Spawn(SpawnEntry entry)
        { 
            var rand = UnityEngine.Random.Range(range.x, range.y);
            Vector3 pos = new Vector3(rand, spawnY, 0f);
            var go = Instantiate(entry.Config.Prefab, pos, Quaternion.identity, parent);

            var item = go.GetComponent<FallingItem>();

            item.Initialize(entry.Config);
            item.OnCaught += HandleCaught;
            item.OnMissed += HandleMissed;

            _activeItems++;
        }

        private void HandleCaught(FallingItem item)
        {
            if (item != null) item.OnCaught -= HandleCaught;
            _activeItems--;
            OnItemCaught?.Invoke(item);
            TryComplete();
        }

        private void HandleMissed(FallingItem item)
        {
            if (item != null) item.OnMissed -= HandleMissed;
            _activeItems--;
            OnItemMissed?.Invoke(item);
            TryComplete();
        }

        private void TryComplete()
        {
            if (_running) return;
            if (_activeItems > 0) return;
            OnSequenceCompleted?.Invoke();
        }
    }
}