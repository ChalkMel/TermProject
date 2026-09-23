using System;
using UnityEngine;

namespace _Source.Resources
{
    public class Credits : MonoBehaviour
    {
        [SerializeField] private int _money = 120;
    
        public Action MoneyChanged;
    
        public int GetMoney()
        {
            return _money;
        }
        
        public void AddMoney(int amount)
        {
            _money += amount;
            MoneyChanged?.Invoke();
        }

        public bool TrySpendMoney(int amount)
        {
            if (_money >= amount)
            {
                SpendMoney(amount);
                return true;
            }
            return false;
        }

        private void SpendMoney(int amount)
        {
            _money -= amount;
            MoneyChanged?.Invoke();
        }
    }
}
