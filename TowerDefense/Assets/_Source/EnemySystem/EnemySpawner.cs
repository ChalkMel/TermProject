using System.Collections;
using System.Collections.Generic;
using _Source.Resources;
using _Source.Waves;
using TMPro;
using UnityEngine;

namespace _Source.EnemySystem
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Credits credits;
        [SerializeField] private WaveGroups waveGroups;
        [SerializeField] private EnemyPath path;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Base baseRef;
        [SerializeField] private TextMeshProUGUI text;

        private int _currentWaveIndex;
        private bool _isSpawning;
        private Coroutine _currentWaveCoroutine;
        private bool _allWavesCompletedInvoked;
        
        private readonly List<EnemyRuntime> _aliveEnemies = new();
        private int _spawnedInWave;
        private bool _waveSpawnFinished;
        
        private int _totalSpawned;
        private int _totalKilled;

        public int TotalSpawned => _totalSpawned;
        public int TotalKilled => _totalKilled;
        public int AliveCount => _aliveEnemies.Count;

        public System.Action<int> OnWaveStarted;
        public System.Action<int> OnWaveCompleted;
        public System.Action OnAllWavesCompleted;
        public System.Action<int, int> OnCountersChanged;

        private void Start()
        {
            StartSpawning();
        }

        public void StartSpawning()
        {
            if (waveGroups == null || waveGroups.WaveCount == 0) return;

            _currentWaveIndex = 0;
            _isSpawning = true;
            _allWavesCompletedInvoked = false;
            _totalSpawned = 0;
            _totalKilled = 0;
            SpawnNextWave();
        }

        public void StopSpawning()
        {
            _isSpawning = false;
            if (_currentWaveCoroutine != null)
            {
                StopCoroutine(_currentWaveCoroutine);
                _currentWaveCoroutine = null;
            }
        }

        private void SpawnNextWave()
        {
            if (!_isSpawning) return;

            text.text = "New Wave";

            if (_currentWaveIndex >= waveGroups.WaveCount)
            {
                CompleteAllWaves();
                return;
            }

            WaveConfig currentWave = waveGroups.GetWave(_currentWaveIndex);
            if (currentWave == null)
            {
                _currentWaveIndex++;
                SpawnNextWave();
                return;
            }
            
            _aliveEnemies.Clear();
            _spawnedInWave = 0;
            _waveSpawnFinished = false;

            OnWaveStarted?.Invoke(_currentWaveIndex);
            _currentWaveCoroutine = StartCoroutine(SpawnWaveRoutine(currentWave));
        }

        private IEnumerator SpawnWaveRoutine(WaveConfig currentWave)
        {
            yield return new WaitForSeconds(currentWave.Delay);

            foreach (EnemyGroup group in currentWave.Groups)
            {
                if (group.enemy == null) continue;

                for (int i = 0; i < group.count; i++)
                {
                    SpawnEnemy(group.enemy);

                    if (i < group.count - 1)
                        yield return new WaitForSeconds(group.interval);
                }
            }

            _waveSpawnFinished = true;
            _currentWaveCoroutine = null;
        }

        private void Update()
        {
            if (!_isSpawning) return;
            
            if (_waveSpawnFinished && _aliveEnemies.Count == 0)
            {
                _waveSpawnFinished = false;
                OnWaveCompleted?.Invoke(_currentWaveIndex);
                _currentWaveIndex++;
                SpawnNextWave();
            }
        }

        private void CompleteAllWaves()
        {
            if (_allWavesCompletedInvoked) return;
            _allWavesCompletedInvoked = true;
            _isSpawning = false;
            OnAllWavesCompleted?.Invoke();
        }

        private void SpawnEnemy(EnemyConfig enemyConfig)
        {
            if (enemyConfig == null || enemyConfig.Prefab == null) return;
            if (spawnPoint == null) return;

            var enemyObj = Instantiate(enemyConfig.Prefab, spawnPoint.position, Quaternion.identity);
            EnemyRuntime enemyRuntime = enemyObj.GetComponent<EnemyRuntime>();
            if (enemyRuntime == null) return;

            enemyRuntime.Initialize(enemyConfig, path, credits, baseRef);
            enemyRuntime.OnEnemyDied += HandleEnemyDied;
            enemyRuntime.OnEnemyReachedBase += HandleEnemyReachedBase;

            _aliveEnemies.Add(enemyRuntime);
            _spawnedInWave++;
            _totalSpawned++;
            OnCountersChanged?.Invoke(_totalKilled, _totalSpawned);
        }

        private void HandleEnemyDied(EnemyRuntime enemy)
        {
            Unsubscribe(enemy);
            _totalKilled++;
            OnCountersChanged?.Invoke(_totalKilled, _totalSpawned);
        }

        private void HandleEnemyReachedBase(EnemyRuntime enemy)
        {
            Unsubscribe(enemy);
        }

        private void Unsubscribe(EnemyRuntime enemy)
        {
            if (enemy == null) return;
            enemy.OnEnemyDied -= HandleEnemyDied;
            enemy.OnEnemyReachedBase -= HandleEnemyReachedBase;
            _aliveEnemies.Remove(enemy);
        }
    }
}