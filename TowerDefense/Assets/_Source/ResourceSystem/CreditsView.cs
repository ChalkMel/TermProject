using Resources;
using TMPro;
using UnityEngine;

namespace Resources
{
    public class CreditsView : MonoBehaviour
    {
        private Credits _credits;
        [SerializeField] private TextMeshProUGUI moneyText;

        public void Init(Credits credits)
        {
            _credits = credits;
            _credits.MoneyChanged += DrawMoney;
            DrawMoney(_credits.GetMoney());
        }
    
        private void OnDestroy()
        {
            _credits.MoneyChanged -= DrawMoney;
        }

        private void DrawMoney(int money)
        {
            moneyText.text = money.ToString();
        }
    }
}
