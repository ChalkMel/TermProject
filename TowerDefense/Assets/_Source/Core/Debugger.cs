using UnityEngine;

namespace Core
{
  public class Debugger : MonoBehaviour
  {
    [SerializeField] private GameObject test;
    
    private void Update()
    {
      if(Input.GetKeyDown(KeyCode.D))
        test.SetActive(!test.activeSelf);
    }
  }
}