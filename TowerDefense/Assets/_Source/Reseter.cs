using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Source
{
  public class Reseter : MonoBehaviour
  {
     private void Update()
    {
      if(Input.GetKeyDown(KeyCode.R))
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }
}