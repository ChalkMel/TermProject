using UnityEngine;

namespace HeatSystem
{
    public class SteamEffect : MonoBehaviour
    {
        [SerializeField] private HeatSystem heatSystem;
        [SerializeField] private GameObject steamObject;
        
        private ParticleSystem _particles;
        private bool _isVisible;

        private void Awake()
        { 
            HideSteam();
        }

        private void OnEnable()
        {
            if (heatSystem == null) return;

            heatSystem.OnOverheated += ShowSteam;
            heatSystem.OnOverheatRecovered += HideSteam;
            if (heatSystem.IsOverheated) ShowSteam();
            else HideSteam();
        }

        private void OnDisable()
        {
            if (heatSystem == null) return;

            heatSystem.OnOverheated -= ShowSteam;
            heatSystem.OnOverheatRecovered -= HideSteam;
        }

        private void ShowSteam()
        {
            if (_isVisible || steamObject == null) return;
            _isVisible = true;
            steamObject.SetActive(true);
        }

        private void HideSteam()
        {
            if (!_isVisible && steamObject != null && !steamObject.activeSelf) return;
            _isVisible = false;
            steamObject.SetActive(false);
        }
    }
}