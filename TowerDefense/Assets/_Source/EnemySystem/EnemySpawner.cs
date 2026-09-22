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
        [SerializeField] private Base baseRef;
        
        private int _currentWaveIndex;
        private bool _isSpawning;
        private Coroutine _currentWaveCoroutine;
        private bool _allWavesCompletedInvoked;

        public System.Action<int> OnWaveStarted;
        public System.Action<int> OnWaveCompleted;
        public System.Action OnAllWavesCompleted;

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

            if (!_isSpawning) yield break;

            int completedWave = _currentWaveIndex;
            _currentWaveIndex++;
            OnWaveCompleted?.Invoke(completedWave);

            if (_currentWaveIndex >= waveGroups.WaveCount)
            {
                CompleteAllWaves();
            }
            else
            {
                yield return new WaitForSeconds(2f);
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
            if (enemyRuntime != null)
            {
                enemyRuntime.Initialize(enemyConfig, path, credits, baseRef);
            }
        }
    }
}