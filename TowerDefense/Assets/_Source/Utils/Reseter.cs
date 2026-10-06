using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
  public class Reseter : MonoBehaviour
  {
    private void Awake()
    {
      Time.timeScale = 1;
    }

    private void Update()
    {
      if(Input.GetKeyDown(KeyCode.R))
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }
}