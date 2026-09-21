using TMPro;
using UnityEngine;

namespace _Source.Resources
{
    public class CreditsUI : MonoBehaviour
    {
        [SerializeField] private Credits credits;
        [SerializeField] private TextMeshProUGUI moneyText;
        private void Awake()
        {
            credits.MoneyChanged += DrawMoney;
            DrawMoney();
        }
    
        private void OnDestroy()
        {
            credits.MoneyChanged -= DrawMoney;
        }

        private void DrawMoney()
        {
            moneyText.text = credits.GetMoney().ToString();
        }
    }
}
