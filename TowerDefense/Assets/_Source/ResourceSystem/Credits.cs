using System;

namespace Resources
{
    public class Credits
    {
        private int _money;

        public Credits(int startingMoney = 120) => _money = startingMoney;

        public event Action<int> MoneyChanged;

        public int GetMoney() => _money;

        public void AddMoney(int amount)
        {
            _money += amount;
            MoneyChanged?.Invoke(_money);
        }

        public bool TrySpendMoney(int amount)
        {
            if (_money < amount) return false;
            _money -= amount;
            MoneyChanged?.Invoke(_money);
            return true;
        }
    }
}