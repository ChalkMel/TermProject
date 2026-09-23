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

        private readonly List<Vector3Int> _spawnCells = new();
        private _Source.Acorn.Acorn _currentAcorn;

        public int Reward => reward;

        private void Start()
        {
            CacheSpawnCells();

            if (_spawnCells.Count == 0)
            {
                Debug.LogWarning("[AcornSpawner] Нет тайлов для спавна. Проверьте Tilemap.");
                return;
            }

            StartCoroutine(SpawnLoop());
        }
        
        private void CacheSpawnCells()
        {
            _spawnCells.Clear();
            if (spawnTilemap == null) return;

            foreach (var pos in spawnTilemap.cellBounds.allPositionsWithin)
            {
                if (spawnTilemap.HasTile(pos))
                    _spawnCells.Add(pos);
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                float timer = spawnInterval;
                while (timer > 0f || _currentAcorn != null)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }

                SpawnAcorn();
            }
        }

        private void SpawnAcorn()
        {
            if (_spawnCells.Count == 0 || acornPrefab == null) return;

            Vector3Int cell = _spawnCells[Random.Range(0, _spawnCells.Count)];
            Vector3 worldPos = spawnTilemap.GetCellCenterWorld(cell);

            _currentAcorn = Instantiate(acornPrefab, worldPos, Quaternion.identity);
            _currentAcorn.Initialize(this, credits, reward);
            _currentAcorn.OnPickedUp += HandleAcornPickedUp;
        }

        private void HandleAcornPickedUp(Acorn acorn)
        {
            if (acorn != null) acorn.OnPickedUp -= HandleAcornPickedUp;
            _currentAcorn = null;
        }
    }
}