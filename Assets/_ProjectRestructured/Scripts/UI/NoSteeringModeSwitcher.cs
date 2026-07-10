using _ProjectRestructured.Scripts.Data;
using _ProjectRestructured.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _ProjectRestructured.Scripts.UI
{
    public class NoSteeringModeSwitcher : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI stateText;
        
        private bool isOn;

        private void Start()
        {
            button.onClick.AddListener(ToggleState);
            isOn = DataManager.playerData.noSteeringMode;
            UpdateState();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(ToggleState);
        }

        private void ToggleState()
        {
            isOn = !isOn;
            UpdateState();
        }

        private void UpdateState()
        {
            stateText.text = isOn ? "On" : "Off";
            VehicleManager.Instance.Current.controller.SetSteeringOnOff(!isOn);
            DataManager.playerData.noSteeringMode = isOn;
        }
    }
}
