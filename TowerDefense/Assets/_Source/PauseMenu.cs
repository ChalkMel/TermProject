using System;
using UnityEngine;

namespace _Source
{
  public class PauseMenu : MonoBehaviour
  {
    [SerializeField] private GameObject PauseScreen;
    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        if (Mathf.Approximately(Time.timeScale, 1)) 
        {
          Time.timeScale = 0;
          PauseScreen.SetActive(true);
        }
        else 
        {
          Time.timeScale = 1;
          PauseScreen.SetActive(false);
        }
      }
    }
  }
}