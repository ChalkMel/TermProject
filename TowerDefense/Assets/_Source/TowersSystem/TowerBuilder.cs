using _Source.Resources;
using _Source.TowersSystem.Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace _Source.TowersSystem
{
  public class TowerBuilder : MonoBehaviour
  {
    [SerializeField] private TowerMenu menu;
    [SerializeField] private Credits credits;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Camera cam;

    [SerializeField] private SpriteRenderer highlighter;

    private Vector3Int _pendingCell;
    private bool _hasPendingCell;

    private void Awake()
    {
      menu.TowerChosen += OnTowerChosen;
      menu.Hide();
    }

    private void OnDestroy()
    {
      menu.TowerChosen -= OnTowerChosen;
    }

    private void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;

        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        TryOpenMenuForCell(cellPos);
      }
    }

    private void TryOpenMenuForCell(Vector3Int cellPos)
    {
      if (tilemap.GetTile(cellPos) != null) return;

      _pendingCell = cellPos;
      _hasPendingCell = true;

      ShowHighlighter(cellPos);
      menu.Show();
    }

    private void OnTowerChosen(TowerConfig tower)
    {
      if (!_hasPendingCell) return;

      if (credits.TrySpendMoney(tower.Cost))
      {
        tilemap.SetTile(_pendingCell, tower.Tile);
      }

      _hasPendingCell = false;
      menu.Hide();
    }

    private void ShowHighlighter(Vector3Int cell)
    {
      if (highlighter == null) return;
      highlighter.gameObject.SetActive(true);
      highlighter.transform.position = tilemap.GetCellCenterWorld(cell);
    }

    private void HideHighlighter()
    {
      if (highlighter != null) highlighter.gameObject.SetActive(false);
    }
  }
}
