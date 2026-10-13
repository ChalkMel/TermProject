using System.Collections.Generic;

namespace Core.Effects
{
  public sealed class ModifierSet
  {
    public enum Combine { Max, Product, Sum }

    private readonly Dictionary<object, float> _values = new();
    private readonly Combine _mode;

    public ModifierSet(Combine mode) => _mode = mode;

    public int Count => _values.Count;
    public bool IsEmpty => _values.Count == 0;

    public void Set(object source, float value) => _values[source] = value;

    public bool Remove(object source) => _values.Remove(source);

    public void Clear() => _values.Clear();

    public float Resolve()
    {
      if (_values.Count == 0) return Default;

      switch (_mode)
      {
        case Combine.Max:
        {
          float best = float.MinValue;
          foreach (var v in _values.Values)
            if (v > best) best = v;
          return best;
        }
        case Combine.Product:
        {
          float acc = 1f;
          foreach (var v in _values.Values) acc *= v;
          return acc;
        }
        case Combine.Sum:
        {
          float acc = 0f;
          foreach (var v in _values.Values) acc += v;
          return acc;
        }
      }
      return Default;
    }

    private float Default => _mode == Combine.Product ? 1f : 0f;
  }
}