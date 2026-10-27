using UnityEngine;
using UnityEngine.Tilemaps;

namespace MiniGames.Balance
{
  public class BalancePath : MonoBehaviour
  {
    [SerializeField] private Tilemap tilemap;
    public Tilemap Tilemap => tilemap;

    public bool IsOnPath(Vector2 worldPos)
    {
      if (tilemap == null) return false;
      return tilemap.HasTile(tilemap.WorldToCell(worldPos));
    }
    
  }
}