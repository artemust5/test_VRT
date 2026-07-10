using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace _Project._Scripts.Nick
{
    public class OculusControllerActivityStatus : MonoBehaviour
    {
        [Header("In Front")]
        public GameObject inFrontLeftControllerActive;
        public GameObject inFrontLeftControllerInactive;
        public GameObject inFrontRightControllerActive;
        public GameObject inFrontRightControllerInactive;

        public float activityInterval = 0.1f;
        public InputData inputData;

        private float nextCheckTime;
        private XRInputModalityManager inputModalityManager;

        private void Start()
        {
            inputModalityManager = FindObjectOfType<XRInputModalityManager>();
            nextCheckTime = Time.time + activityInterval;
        }

        private void Update()
        {
            if (!(Time.time >= nextCheckTime)) return;
            CheckDevicesStatus();
            nextCheckTime = Time.time + activityInterval;
        }

        private void CheckDevicesStatus()
        {
            if (!inputModalityManager.isActiveAndEnabled) return;
            
            var leftActive = inputModalityManager.leftController.activeInHierarchy || IsControllerActive(inputData.LeftController);
            var rightActive = inputModalityManager.rightController.activeInHierarchy || IsControllerActive(inputData.RightController);
            
            inFrontLeftControllerActive.SetActive(leftActive);
            inFrontLeftControllerInactive.SetActive(!leftActive);
            
            inFrontRightControllerActive.SetActive(rightActive);
            inFrontRightControllerInactive.SetActive(!rightActive);
        }

        private static bool IsControllerActive(InputDevice device)
        {
            if (!device.isValid) return false;

            return device.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue) && triggerValue > 0.1f;
        }
    }
}
