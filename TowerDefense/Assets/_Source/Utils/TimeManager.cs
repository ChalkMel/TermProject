using Resources;
using ResourceSystem;
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
    [SerializeField] private Button addManaButton;

    private Credits _credits;
    private Mana _mana;
    public void Init( Credits credits, Mana mana)
    {
      _credits = credits;
      _mana = mana;
      upButton.onClick.AddListener(AddTime);
      downButton.onClick.AddListener(RemoveTime);
      addButton.onClick.AddListener(AddMoney);
      addManaButton.onClick.AddListener(AddMana);
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
      _credits.AddMoney(20);
    }

    private void AddMana()
    {
      _mana.Add(10);
    }
  }
}