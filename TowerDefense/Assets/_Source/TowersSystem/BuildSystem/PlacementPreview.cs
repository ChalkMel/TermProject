using TowersSystem.Config;
using UnityEngine;

namespace TowersSystem.BuildSystem
{
  public class PlacementPreview : MonoBehaviour
  {
    [SerializeField] private Color rangeColor   = new(0.2f, 0.8f, 1f, 0.6f);
    [SerializeField] private Color validColor   = new(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color invalidColor = new(1f, 0.4f, 0.4f, 0.6f);

    private RangeIndicator _range;
    private SpriteRenderer _sprite;

    private void Awake()
    {
      var rangeGo = new GameObject("PreviewRange");
      rangeGo.transform.SetParent(transform, false);
      _range = rangeGo.AddComponent<RangeIndicator>();
      _range.SetColor(rangeColor);

      var spriteGo = new GameObject("PreviewSprite");
      spriteGo.transform.SetParent(transform, false);
      _sprite = spriteGo.AddComponent<SpriteRenderer>();
      _sprite.color = validColor;
      _sprite.sortingOrder = 99;
      _sprite.enabled = false;
    }

    public void Show(TowerConfig config, Vector3 worldPos, Vector3 cellSize, bool valid = true)
    {
      _range.Show(worldPos, config.Range);

      if (config.Tile?.sprite == null) { _sprite.enabled = false; return; }

      _sprite.sprite = config.Tile.sprite;
      _sprite.transform.position = worldPos;
      _sprite.color = valid ? validColor : invalidColor;
      _sprite.enabled = true;

      var s = config.Tile.sprite.bounds.size;
      _sprite.transform.localScale = new Vector3(
        s.x > 0 ? cellSize.x / s.x : 1f,
        s.y > 0 ? cellSize.y / s.y : 1f, 1f);
    }

    public void Hide()
    {
      _range.Hide();
      _sprite.enabled = false;
    }
  }
}