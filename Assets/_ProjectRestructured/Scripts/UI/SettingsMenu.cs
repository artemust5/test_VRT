using System.Globalization;
using _ProjectRestructured.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _ProjectRestructured.Scripts.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        public Button increaseSteeringResponsivenessButton;
        public Button decreaseSteeringResponsivenessButton;
        public Button increaseSteeringAngleMultiplierButton;
        public Button decreaseSteeringAngleMultiplierButton;
        public Button increaseMotorForceButton;
        public Button decreaseMotorForceButton;
        public Button increaseBrakeForceButton;
        public Button decreaseBrakeForceButton;

        public TextMeshProUGUI steeringResponsivenessText;
        public TextMeshProUGUI steeringAngleMultiplierText;
        public TextMeshProUGUI motorForceText;
        public TextMeshProUGUI brakeForceText;


        private void Start()
        {
            SubscribeButtons();
            UpdateUI();
        }

        private void SubscribeButtons()
        {
            increaseSteeringResponsivenessButton?.onClick.AddListener(() => AdjustSteeringResponsiveness(true));
            decreaseSteeringResponsivenessButton?.onClick.AddListener(() => AdjustSteeringResponsiveness(false));
            increaseSteeringAngleMultiplierButton?.onClick.AddListener(() => AdjustSteeringAngleMultiplier(true));
            decreaseSteeringAngleMultiplierButton?.onClick.AddListener(() => AdjustSteeringAngleMultiplier(false));
            increaseMotorForceButton?.onClick.AddListener(() => AdjustMotorForce(true));
            decreaseMotorForceButton?.onClick.AddListener(() => AdjustMotorForce(false));
            increaseBrakeForceButton?.onClick.AddListener(() => AdjustBrakeForce(true));
            decreaseBrakeForceButton?.onClick.AddListener(() => AdjustBrakeForce(false));

            DataManager.onDataUpdated += UpdateUI;
        }

        private void OnDestroy()
        {
            increaseSteeringResponsivenessButton?.onClick.RemoveAllListeners();
            decreaseSteeringResponsivenessButton?.onClick.RemoveAllListeners();
            increaseSteeringAngleMultiplierButton?.onClick.RemoveAllListeners();
            decreaseSteeringAngleMultiplierButton?.onClick.RemoveAllListeners();
            increaseMotorForceButton?.onClick.RemoveAllListeners();
            decreaseMotorForceButton?.onClick.RemoveAllListeners();
            increaseBrakeForceButton?.onClick.RemoveAllListeners();
            decreaseBrakeForceButton?.onClick.RemoveAllListeners();
            
            DataManager.onDataUpdated -= UpdateUI;
        }

        private void UpdateUI()
        {
            steeringResponsivenessText.text = DataManager.CurrentSteeringResponsiveness.ToString(CultureInfo.CurrentCulture);
            steeringAngleMultiplierText.text = DataManager.CurrentSteeringAngleMultiplier.ToString(CultureInfo.CurrentCulture);
            motorForceText.text = DataManager.CurrentMotorForce.ToString(CultureInfo.CurrentCulture);
            brakeForceText.text = DataManager.CurrentBrakeForce.ToString(CultureInfo.CurrentCulture);
        }
        
        private void AdjustSteeringResponsiveness(bool increase)
        {
            var value = DataManager.CurrentSteeringResponsiveness;
            DataManager.CurrentSteeringResponsiveness = increase switch
            {
                true when value < 20f => value + 1f,
                false when value > 0f => value - 1f,
                _ => value
            };

            UpdateUI();
        }
        private void AdjustSteeringAngleMultiplier(bool increase)
        {
            var value = DataManager.CurrentSteeringAngleMultiplier;
            DataManager.CurrentSteeringAngleMultiplier = increase switch
            {
                true when value < 20f => value + 1f,
                false when value > 0f => value - 1f,
                _ => value
            };

            UpdateUI();
        }
        private void AdjustMotorForce(bool increase)
        {
            var value = DataManager.CurrentMotorForce;
            DataManager.CurrentMotorForce = increase switch
            {
                true when value < 20f => value + 1f,
                false when value > 0f => value - 1f,
                _ => value
            };

            UpdateUI();
        }
        private void AdjustBrakeForce(bool increase)
        {
            var value = DataManager.CurrentBrakeForce;
            DataManager.CurrentBrakeForce = increase switch
            {
                true when value < 5f => value + 1f,
                false when value > 0f => value - 1f,
                _ => value
            };

            UpdateUI();
        }
    }
}
