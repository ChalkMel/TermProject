using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceSystem
{
    public class ManaView : MonoBehaviour
    {
        [SerializeField] private Mana mana;
        [SerializeField] private Image fillBar;
        [SerializeField] private TextMeshProUGUI manaText;

        public void Init(Mana manaRef)
        {
            mana = manaRef;
            if (mana == null) return;
            mana.ManaChanged += Draw;
            Draw(mana.Current, mana.Max);
        }

        private void OnDestroy()
        {
            if (mana != null) mana.ManaChanged -= Draw;
        }

        private void Draw(float current, float max)
        {
            if (fillBar != null) fillBar.fillAmount = max > 0f ? current / max : 0f;
            if (manaText != null) manaText.text = $"{Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
        }
    }
}