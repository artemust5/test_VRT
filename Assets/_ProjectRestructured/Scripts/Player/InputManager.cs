using System;
using _Project._Scripts;
using _Project._Scripts.Nick;
using _ProjectRestructured.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace _ProjectRestructured.Scripts.Player
{
    public class InputManager : MonoBehaviour
    {
        public bool loadInputFromDataManager;
        public bool noSteeringMode;
        public bool debug;
        [SerializeField] private InputMode inputMode;
        private NoSteeringController steeringController;
        private InputData vrControllersInputData;
        
        //RealBicycleParameters
        private float xrControllerCenteredPointForSteer = 290f;
        private float prevXrSteerValue;
        private Quaternion prevXrAccelerationControllerRotation;
        
        // Debug Text Components
        [Header("Debug Text Fields")]
        // Steering
        [SerializeField] private Text currentAngleYText;
        [SerializeField] private Text centeredPointText;
        [SerializeField] private Text deltaAngleText;
        [SerializeField] private Text rawSteeringValueText;
        [SerializeField] private Text steeringMultiplierText;
        [SerializeField] private Text finalSteeringValueText;
        [SerializeField] private Text thresholdStatusText;
        // Speed
        [SerializeField] private Text currentSpeedControllerAngleText;
        [SerializeField] private Text speedMultiplierText;
        [SerializeField] private Text finalSpeedValueText;

        private void Start()
        {
            if (!debug)
            {
                finalSpeedValueText.transform.parent.parent.gameObject.SetActive(false);
            }

            steeringController = FindObjectOfType<NoSteeringController>();
            vrControllersInputData = GetComponent<InputData>();

            DataManager.onDataUpdated += ChangeInputMode;
            XRControllerReset.onXrResetPerformed += ResetXRControllerCenteredPoint;
            if (!loadInputFromDataManager) return;
            inputMode = DataManager.playerData.inputMode;
            steeringController.enabled = noSteeringMode || DataManager.playerData.noSteeringMode;
        }

        private void OnDestroy()
        {
            XRControllerReset.onXrResetPerformed -= ResetXRControllerCenteredPoint;
            DataManager.onDataUpdated -= ChangeInputMode;
        }
        
        private void FixedUpdate()
        {
            SetInputs();
        }

        private void SetInputs()
        {
            switch (inputMode)
            {
                case InputMode.Keyboard:
                    SetKeyboardInputs();
                    break;
                case InputMode.VRControllers:
                    SetVRControllersInputs();
                    break;
                case InputMode.RealBicycle:
                    SetRealBicycleControllersInputs();
                    break;
                case InputMode.EScooter:
                    SetEScooterInputs();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetKeyboardInputs()
        {
            // Steer
            VehicleManager.Instance.Current.controller.SetCustomSteerInput(Input.GetAxisRaw("Horizontal"));
            
            // Acceleration
            VehicleManager.Instance.Current.controller.SetCustomAccelerationInput(Input.GetAxisRaw("Vertical"), 15f, 1f);
            
            // Brake
            VehicleManager.Instance.Current.controller.SetCustomBrakeInput(Input.GetKey(KeyCode.Space) ? 1f : 0f);
            
            // Additional
            VehicleManager.Instance.Current.controller.ActivateAdditionalKeyboardButtons();
        }
        
        private void SetVRControllersInputs()
        {
            // Steer
            vrControllersInputData.RightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out var stickInput);
            var steerValue = stickInput.x;
            VehicleManager.Instance.Current.controller.SetCustomSteerInput(steerValue*DataManager.playerData.steeringAngleMultiplierForControllers/10, DataManager.playerData.steeringResponsivenessForControllers/2);
            
            // Acceleration
            vrControllersInputData.RightController.TryGetFeatureValue(CommonUsages.trigger, out var accelerationValue);
            VehicleManager.Instance.Current.controller.SetCustomAccelerationInput(accelerationValue, DataManager.playerData.motorForceForControllers, 1f);
            
            // Brake
            vrControllersInputData.RightController.TryGetFeatureValue(CommonUsages.grip, out var brakeValue);
            VehicleManager.Instance.Current.controller.SetCustomBrakeInput(brakeValue*DataManager.playerData.brakeForceForControllers/5f);
        }
        
        private void SetEScooterInputs()
        {
            // Steer
            VehicleManager.Instance.Current.controller.SetCustomSteerInput(CalculateHorizontalInputFromRightController(), DataManager.playerData.steeringResponsivenessForBicycle/2);
            
            // Acceleration
            vrControllersInputData.RightController.TryGetFeatureValue(CommonUsages.trigger, out var accelerationValue);
            VehicleManager.Instance.Current.controller.SetCustomAccelerationInput(accelerationValue, DataManager.playerData.motorForceForControllers, 1f);
            
            // Brake
            vrControllersInputData.LeftController.TryGetFeatureValue(CommonUsages.grip, out var brakeValue);
            VehicleManager.Instance.Current.controller.SetCustomBrakeInput(brakeValue*DataManager.playerData.brakeForceForControllers/5f);
        }
        private void SetRealBicycleControllersInputs()
        {
            // Steer
            VehicleManager.Instance.Current.controller.SetCustomSteerInput(CalculateHorizontalInputFromRightController(), DataManager.playerData.steeringResponsivenessForBicycle/2);
            
            // Acceleration
            VehicleManager.Instance.Current.controller.SetCustomAccelerationInput(CalculateVerticalInputFromLeftControllerQuaternion(), DataManager.playerData.motorForceForBicycle, 1f);
            
            // Brake
            vrControllersInputData.RightController.TryGetFeatureValue(CommonUsages.grip, out var brakeValue);
            VehicleManager.Instance.Current.controller.SetCustomBrakeInput(brakeValue*DataManager.playerData.brakeForceForBicycle/5f);
        }

        private float CalculateVerticalInputFromLeftControllerQuaternion()
        {
            const float pedalRotationThreshold  = 10f;
            var rotationValue = 0f;
            
            vrControllersInputData.LeftController.TryGetFeatureValue(CommonUsages.deviceRotation, out var currentLeftControllerRotation);
            
            var angleDifference = Mathf.DeltaAngle(prevXrAccelerationControllerRotation.eulerAngles.z, currentLeftControllerRotation.eulerAngles.z);
            prevXrAccelerationControllerRotation = currentLeftControllerRotation;
            
            switch (DataManager.playerData.inverseAxisValue ? -angleDifference : angleDifference)
            {
                case < -1f: // forward
                    rotationValue = Mathf.Clamp01(Mathf.Abs(angleDifference) / pedalRotationThreshold );
                    break;

                case > 2f: // backward
                    break;
            }

            if (debug)
                UpdateSpeedDebugText(currentLeftControllerRotation.eulerAngles.z,DataManager.playerData.motorForceForBicycle, rotationValue);
            return rotationValue;
        }

        private float CalculateHorizontalInputFromRightController()
        {
            const float maxSteeringAngleThreshold = 80f;
            const float steeringAngleNormalizationFactor = maxSteeringAngleThreshold/4;
            const float steeringAngleNormalizationDivisor = 1f/20;
            
            vrControllersInputData.RightController.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation);
            var currentAngleY = rotation.eulerAngles.y;
                    
            var deltaAngle = Mathf.DeltaAngle(xrControllerCenteredPointForSteer, currentAngleY);
            var rawSteeringValue = deltaAngle/steeringAngleNormalizationFactor;
            var steeringMultiplier = DataManager.playerData.steeringAngleMultiplierForBicycle * steeringAngleNormalizationDivisor;
            var currentValue = Mathf.Clamp(rawSteeringValue * steeringMultiplier,-1,1);
            
            var isWithinThreshold = Mathf.Abs(deltaAngle) <= maxSteeringAngleThreshold;
            
            if (debug) 
                UpdateSteeringDebugText(currentAngleY, deltaAngle, rawSteeringValue, currentValue);
            
            if (!isWithinThreshold) return prevXrSteerValue;
            
            prevXrSteerValue = currentValue;
            return currentValue;
        }
        
        public void ResetXRControllerCenteredPoint()
        {
            vrControllersInputData.RightController.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation);
            xrControllerCenteredPointForSteer = rotation.eulerAngles.y;
        }
        
        private void ChangeInputMode()
        {
            inputMode = DataManager.playerData.inputMode;
        }
        
        private void UpdateSteeringDebugText(float currentAngleY, float deltaAngle, float rawSteeringValue, float finalValue)
        {
            if (currentAngleYText == null || centeredPointText == null || deltaAngleText == null ||
                rawSteeringValueText == null || steeringMultiplierText == null || finalSteeringValueText == null ||
                thresholdStatusText == null)
            {
                Debug.LogError("Objects aren't attached!");
                return;
            }
            currentAngleYText.text = $"{currentAngleY:F2}°";
            centeredPointText.text = $"{xrControllerCenteredPointForSteer:F2}°";
            deltaAngleText.text = $"{deltaAngle:F2}°";
            rawSteeringValueText.text = $"{rawSteeringValue:F2}";
            
            const float steeringAngleNormalizationDivisor = 1f/20;
            steeringMultiplierText.text = $"{DataManager.playerData.steeringAngleMultiplierForBicycle * steeringAngleNormalizationDivisor:F2}";
            finalSteeringValueText.text = $"{finalValue:F2}";
            thresholdStatusText.text = $"({Mathf.Abs(deltaAngle):F2}° / 80°)";
        }
        private void UpdateSpeedDebugText(float currentSpeedControllerAngle, float speedMultiplier, float finalSpeedValue)
        {
            if (currentSpeedControllerAngleText == null ||  speedMultiplierText == null || finalSpeedValueText == null)
            {
                Debug.LogError("Objects aren't attached!");
                return;
            }
            currentSpeedControllerAngleText.text = $"{currentSpeedControllerAngle:F2}°";
            speedMultiplierText.text = $"{speedMultiplier:F1}°";
            finalSpeedValueText.text = $"{finalSpeedValue:F2}°";
        }
    }
}
