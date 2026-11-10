using System.Collections;
using System.Collections.Generic;
using LevelSystem;
using Resources;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AcornSystem
{
  public class AcornSpawner : MonoBehaviour
  {
    [Header("Refs")]
    [SerializeField] private Tilemap spawnTilemap;
    [SerializeField] private Acorn acornPrefab;
    private Credits _credits;
    private DifficultyConfig _difficulty;

    [Header("Settings")]
    [SerializeField] private float spawnInterval = 8f;
    [SerializeField] private int reward = 25;
    [SerializeField] private int maxAcorns = 3;

    private readonly List<Vector3Int> _spawnCells = new();
    private readonly List<Acorn> _activeAcorns = new();

    public void Init(Credits credits, DifficultyConfig difficulty = null)
    {
      _credits = credits;
      _difficulty = difficulty;
      CacheSpawnCells();

      if (_spawnCells.Count == 0) return;
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
      float intervalMult = _difficulty?.AcornSpawnIntervalMultiplier ?? 1f;

      while (true)
      {
        yield return new WaitForSeconds(spawnInterval * intervalMult);

        _activeAcorns.RemoveAll(a => a == null);

        if (_activeAcorns.Count < maxAcorns)
          SpawnAcorn();
      }
    }

    private void SpawnAcorn()
    {
      if (_spawnCells.Count == 0 || acornPrefab == null) return;

      Vector3Int cell   = PickFreeCell();
      Vector3 worldPos  = spawnTilemap.GetCellCenterWorld(cell);

      var acorn = Instantiate(acornPrefab, worldPos, Quaternion.identity);

      float rewMult = _difficulty?.AcornRewardMultiplier ?? 1f;
      int scaledReward = Mathf.RoundToInt(reward * rewMult);

      acorn.Initialize(this, _credits, scaledReward);
      acorn.OnPickedUp += HandleAcornPickedUp;
      _activeAcorns.Add(acorn);
    }

    private Vector3Int PickFreeCell()
    {
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

      return _spawnCells[Random.Range(0, _spawnCells.Count)];
    }

    private void HandleAcornPickedUp(Acorn acorn)
    {
      if (acorn != null) acorn.OnPickedUp -= HandleAcornPickedUp;
      _activeAcorns.Remove(acorn);
    }
  }
}