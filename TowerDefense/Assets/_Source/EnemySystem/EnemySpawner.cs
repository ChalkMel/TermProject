using System;
using System.Collections;
using _Source.Resources;
using _Source.Waves;
using UnityEngine;

namespace _Source.EnemySystem
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Credits credits;
        [SerializeField] private WaveGroups waveGroups;
        [SerializeField] private EnemyPath path;
        [SerializeField] private Transform spawnPoint;
        
        private int _currentWaveIndex;
        private bool _isSpawning;
        private Coroutine _currentWaveCoroutine;

        public System.Action<int> OnWaveStarted;
        public System.Action<int> OnWaveCompleted;
        public System.Action OnAllWavesCompleted;

        private void Awake()
        {
            StartSpawning();
        }

        public void StartSpawning()
        {
            _currentWaveIndex = 0;
            _isSpawning = true;
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
            if (_currentWaveIndex >= waveGroups.WaveCount)
            {
                _isSpawning = false;
                OnAllWavesCompleted?.Invoke();
                return;
            }

            WaveConfig currentWave = waveGroups.GetWave(_currentWaveIndex);
            if (currentWave == null) return;

            _isSpawning = true;
            OnWaveStarted?.Invoke(_currentWaveIndex);
            _currentWaveCoroutine = StartCoroutine(SpawnWaveCoroutine(currentWave));
        }

        private IEnumerator SpawnWaveCoroutine(WaveConfig wave)
        {
            yield return new WaitForSeconds(wave.Delay);

            for (int i = 0; i < wave.Count; i++)
            {
                if (!_isSpawning) yield break;
                
                SpawnEnemy(wave.Enemy);
                yield return new WaitForSeconds(wave.Interval);
            }

            _currentWaveIndex++;
            OnWaveCompleted?.Invoke(_currentWaveIndex - 1);
            
            if (_isSpawning && _currentWaveIndex < waveGroups.WaveCount)
            {
                yield return new WaitForSeconds(2f);
                SpawnNextWave();
            }
            else if (_currentWaveIndex >= waveGroups.WaveCount)
            {
                _isSpawning = false;
                OnAllWavesCompleted?.Invoke();
            }
        }

        private void SpawnEnemy(EnemyConfig enemyConfig)
        {
            if (enemyConfig == null || enemyConfig.Prefab == null) return;
            
            var enemyObj = Instantiate(enemyConfig.Prefab, spawnPoint.position, Quaternion.identity);
            EnemyRuntime enemyRuntime = enemyObj.GetComponent<EnemyRuntime>();
            enemyRuntime.Initialize(enemyConfig, path, credits);
        }
    }
}