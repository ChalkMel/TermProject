using System.Collections;
using System.Collections.Generic;
using _Source.Resources;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Source.Acorn
{
    public class AcornSpawner : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Tilemap spawnTilemap;
        [SerializeField] private Acorn acornPrefab;
        [SerializeField] private Credits credits;

        [Header("Settings")]
        [SerializeField] private float spawnInterval = 8f;
        [SerializeField] private int reward = 25;
        [SerializeField] private int maxAcorns = 3;

        private readonly List<Vector3Int> _spawnCells = new();
        private readonly List<Acorn> _activeAcorns = new();

        public int Reward => reward;
        public int ActiveCount => _activeAcorns.Count;
        public int MaxAcorns => maxAcorns;

        private void Start()
        {
            CacheSpawnCells();

            if (_spawnCells.Count == 0)
            {
                Debug.LogWarning("[AcornSpawner] Нет тайлов для спавна.");
                return;
            }

            StartCoroutine(SpawnLoop());
        }

        private void CacheSpawnCells()
        {
            _spawnCells.Clear();
            if (spawnTilemap == null) return;

            foreach (var pos in spawnTilemap.cellBounds.allPositionsWithin)
                if (spawnTilemap.HasTile(pos))
                    _spawnCells.Add(pos);
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);

                // Чистим мёртвые ссылки (на всякий случай)
                _activeAcorns.RemoveAll(a => a == null);

                // Спавним только если есть свободный слот
                if (_activeAcorns.Count < maxAcorns)
                    SpawnAcorn();
            }
        }

        private void SpawnAcorn()
        {
            if (_spawnCells.Count == 0 || acornPrefab == null) return;

            // Ищем клетку, где ещё нет жёлудя (по желанию — можно убрать)
            Vector3Int cell = PickFreeCell();
            Vector3 worldPos = spawnTilemap.GetCellCenterWorld(cell);

            var acorn = Instantiate(acornPrefab, worldPos, Quaternion.identity);
            acorn.Initialize(this, credits, reward);
            acorn.OnPickedUp += HandleAcornPickedUp;

            _activeAcorns.Add(acorn);
        }

        private Vector3Int PickFreeCell()
        {
            // Пытаемся не ставить два жёлудя в одну клетку
            const int maxTries = 16;
            for (int i = 0; i < maxTries; i++)
            {
                var candidate = _spawnCells[Random.Range(0, _spawnCells.Count)];
                bool occupied = false;

                foreach (var a in _activeAcorns)
                {
                    if (a == null) continue;
                    if (spawnTilemap.WorldToCell(a.transform.position) == candidate)
                    {
                        occupied = true;
                        break;
                    }
                }

                if (!occupied) return candidate;
            }

            // Если не нашли — возвращаем любую (лучше, чем ничего)
            return _spawnCells[Random.Range(0, _spawnCells.Count)];
        }

        private void HandleAcornPickedUp(Acorn acorn)
        {
            if (acorn != null) acorn.OnPickedUp -= HandleAcornPickedUp;
            _activeAcorns.Remove(acorn);
        }
    }
}