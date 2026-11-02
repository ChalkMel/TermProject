using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
  public class SceneSystem : MonoBehaviour
  {
    public void ReturnToMainMenu()
    {
      SceneManager.LoadScene("Menu");
    }
  }
}