using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Source.TowersSystem
{
    public class TowerTooltip : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private TextMeshProUGUI sellPriceText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button sellButton;

        private TowerRuntime _currentTower;
        private TowerBuilder _builder;

        private void Awake()
        {
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
            sellButton.onClick.AddListener(OnSellClicked);
            Hide();
        }

        public void SetBuilder(TowerBuilder builder)
        {
            _builder = builder;
        }

        public void Show(TowerRuntime tower)
        {
            _currentTower = tower;
            gameObject.SetActive(true);
            
            infoText.text = $"<b>{tower.Config.Name}</b> (Lv. {tower.Level})\n" +
                            $"Damage: {tower.Damage}\n" +
                            $"Range: {tower.Range}\n" +
                            $"Cooldown: {tower.Config.Cooldown}s";

            upgradeCostText.text = $"Upgrade: {tower.GetUpgradeCost()}$";
            sellPriceText.text = $"Sell: {tower.GetSellPrice()}$";
        }

        public void Hide()
        {
            _currentTower = null;
            gameObject.SetActive(false);
        }

        private void OnUpgradeClicked()
        {
            if (_currentTower != null && _builder != null)
            {
                _builder.UpgradeTower(_currentTower);
                //Hide(); 
            }
        }

        private void OnSellClicked()
        {
            if (_currentTower != null && _builder != null)
            {
                _builder.SellTower(_currentTower);
                Hide();
            }
        }
    }
}