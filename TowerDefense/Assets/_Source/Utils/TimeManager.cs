using Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
  public class TimeManager : MonoBehaviour
  {
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button addButton;
    [SerializeField] private Credits credits;

    private void Awake()
    {
      upButton.onClick.AddListener(() => AddTime());
      downButton.onClick.AddListener(() => RemoveTime());
      addButton.onClick.AddListener(() => AddMoney());
      UpdateTime();
    }

    private void AddTime()
    {
      if (Time.timeScale > 0f && Time.timeScale < 6f)
      {
        Time.timeScale += 0.5f;
        UpdateTime();
      }
    }
    private void RemoveTime()
    {
      if (Time.timeScale >= 1f)
      {
        Time.timeScale -= 0.5f;
        UpdateTime();
      }
    }

    private void UpdateTime()
    {
      timeText.text = Time.timeScale.ToString();
    }
    
    private void AddMoney()
    {
      credits.AddMoney(5);
    }
  }
}