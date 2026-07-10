using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Nick
{
    public class ChooseAxisForLeftController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textStatus;
        
        private static readonly LeftControllerAxis[] axisValues = { LeftControllerAxis.X, LeftControllerAxis.Y, LeftControllerAxis.Z };
        private int currentAxisIndex;
        private BicycleVehicle bicycleVehicle;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            bicycleVehicle = FindObjectOfType<BicycleVehicle>();
            
            if (textStatus == null)
                Debug.LogError($"TextStatus not found on {gameObject.name}");
                
            if (bicycleVehicle == null)
                Debug.LogError($"BicycleVehicle not found");

            currentAxisIndex = PlayerPrefs.HasKey("axisForLeftController")
                ? PlayerPrefs.GetInt("axisForLeftController")
                : 2;
            button.onClick.AddListener(ChangeAxisValue);
            UpdateAxis();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(ChangeAxisValue);
        }

        private void ChangeAxisValue()
        {
            currentAxisIndex = (currentAxisIndex + 1) % axisValues.Length;
            UpdateAxis();
        }

        private void UpdateAxis()
        {
            var currentAxis = axisValues[currentAxisIndex];
            bicycleVehicle.SetAxisForLeftController(currentAxis);
            textStatus.text = currentAxis switch
            {
                LeftControllerAxis.X => "x",
                LeftControllerAxis.Y => "y",
                LeftControllerAxis.Z => "z",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            PlayerPrefs.SetInt("axisForLeftController", currentAxisIndex);
            PlayerPrefs.Save();
        }
    }
}