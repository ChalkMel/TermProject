using UnityEngine;

namespace MiniGames.WolfAndEggs
{
  [CreateAssetMenu(fileName = "FallingItem", menuName = "WolfAndEggs/Falling Item Config")]
  public class FallingItemConfig : ScriptableObject
  {
    [field: SerializeField] public string DisplayName { get; private set; } = "Item";
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Color Tint { get; private set; } = Color.white;
    [field: SerializeField] public float FallSpeed { get; private set; } = 5f;
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [Header("On Catch (волк поймал)")]
    [field: SerializeField] public int CatchScore { get; private set; } = 1;
    [field: SerializeField] public int CatchHealthDelta { get; private set; } = 0;

    [Header("On Miss (упало мимо)")]
    [field: SerializeField] public int MissScore { get; private set; } = 0;
    [field: SerializeField] public int MissHealthDelta { get; private set; } = 0;
  }
}