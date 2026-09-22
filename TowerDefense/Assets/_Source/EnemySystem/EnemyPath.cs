using System.Collections.Generic;
using UnityEngine;

namespace _Source.EnemySystem
{
  public class EnemyPath : MonoBehaviour
  {
    [SerializeField] private List<Vector2> points = new List<Vector2>();
        
    public List<Vector2> Points => points;
    public int PointCount => points.Count;
        
    public Vector2 GetPoint(int index)
    {
      if (index >= 0 && index < points.Count)
        return points[index];
      return Vector2.zero;
    }
        
    public Vector2 GetNextPoint(int currentIndex)
    {
      if (currentIndex + 1 < points.Count)
        return points[currentIndex + 1];
      return points[points.Count - 1];
    }
        
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      if (points.Count < 2) return;
            
      Gizmos.color = Color.green;
      for (int i = 0; i < points.Count - 1; i++)
      {
        Gizmos.DrawLine(points[i], points[i + 1]);
        Gizmos.DrawSphere(points[i], 0.2f);
      }
      Gizmos.DrawSphere(points[points.Count - 1], 0.2f);
    }
#endif
  }
}