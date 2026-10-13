using TMPro;
using UnityEngine;

namespace Resources
{
    public class CreditsView : MonoBehaviour
    {
        [SerializeField] private Credits credits;
        [SerializeField] private TextMeshProUGUI moneyText;
        private void Awake()
        {
            credits.MoneyChanged += DrawMoney;
        }
    
        private void OnDestroy()
        {
            credits.MoneyChanged -= DrawMoney;
        }

        private void DrawMoney(int money)
        {
            moneyText.text = money.ToString();
        }
    }
}
