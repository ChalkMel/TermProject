using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniGames.TapTapDash
{
  public class TapTapDashPath : MonoBehaviour
  {
    [Tooltip("Tilemap, на котором нарисован уровень.")]
    [SerializeField] private Tilemap tilemap;

    public bool IsOnPath(Vector2 worldPos)
    {
      if (tilemap == null) return false;
      Vector3Int cell = tilemap.WorldToCell(worldPos);
      return tilemap.HasTile(cell);
    }
  }
}