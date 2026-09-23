using System;
using UnityEngine;

namespace _Source
{
  public class PauseMenu : MonoBehaviour
  {
    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        if (Mathf.Approximately(Time.timeScale, 1)) 
        {
          Time.timeScale = 0;
        }
        else 
        {
          Time.timeScale = 1;
        }
      }
    }
  }
}