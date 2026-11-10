using System.Collections.Generic;
using UnityEngine;

namespace LevelSystem
{
  public static class LevelProgress
  {
    private const string CompletedSuffix = "_Completed";

    private static string Key(LevelConfig level) =>
      "Level_" + level.name + CompletedSuffix;

    public static bool IsCompleted(LevelConfig level)
    {
      if (level == null) return false;
      return PlayerPrefs.GetInt(Key(level), 0) == 1;
    }
    
    public static bool IsUnlocked(LevelConfig level, IList<LevelConfig> orderedLevels)
    {
      if (level == null || orderedLevels == null) return false;

      int idx = IndexOf(level, orderedLevels);
      if (idx < 0) return false;
      
      if (idx == 0) return true;
      
      if (IsCompleted(orderedLevels[idx - 1])) return true;
      
      return IsCompleted(level);
    }
    
    public static int FirstUnlockedIndex(IList<LevelConfig> orderedLevels)
    {
      if (orderedLevels == null) return -1;
      for (int i = 0; i < orderedLevels.Count; i++)
        if (IsUnlocked(orderedLevels[i], orderedLevels)) return i;
      return -1;
    }

    public static void MarkCompleted(LevelConfig level)
    {
      if (level == null) return;
      PlayerPrefs.SetInt(Key(level), 1);
      PlayerPrefs.Save();
    }

    public static void ResetLevel(LevelConfig level) //это для меня
    {
      if (level == null) return;
      PlayerPrefs.DeleteKey(Key(level));
      PlayerPrefs.Save();
    }

    public static void ResetAll(IEnumerable<LevelConfig> levels)
    {
      if (levels == null) return;
      foreach (var l in levels)
        if (l != null) PlayerPrefs.DeleteKey(Key(l));
      PlayerPrefs.Save();
    }

    private static int IndexOf(LevelConfig level, IList<LevelConfig> list)
    {
      for (int i = 0; i < list.Count; i++)
        if (list[i] == level) return i;
      return -1;
    }
  }
}