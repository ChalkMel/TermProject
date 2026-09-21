using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerBuilder : MonoBehaviour
{
  [SerializeField] private TowerMenu menu;
  [SerializeField] private Credits credits;
  [SerializeField] private Tilemap tilemap;
  [SerializeField] private Camera cam;

  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      if(EventSystem.current.IsPointerOverGameObject()) return; 
      Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
      worldPos.z = 0;
      
      Vector3Int cellPos = tilemap.WorldToCell(worldPos);

      TryPlaceTile(cellPos);
    }
  }

  private void TryPlaceTile(Vector3Int cellPos)
  {
    TileBase existing = tilemap.GetTile(cellPos);

    if (credits.TrySpendMoney(menu.CurrentTower.Cost))
    {
      if (existing == null)
      {
        tilemap.SetTile(cellPos, menu.CurrentTower.tile);
      }
    }
  }
  
}
