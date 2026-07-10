using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace _Project._Scripts
{
    public class InputData : MonoBehaviour
    {
        public InputDevice RightController;
        public InputDevice LeftController;
        public InputDevice Hmd;


        private void Update()
        {
            if (!RightController.isValid || !LeftController.isValid || !Hmd.isValid)
                InitializeInputDevices();
        }

        private void InitializeInputDevices()
        {
            if (!RightController.isValid)
                InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right,
                    ref RightController);
            if (!LeftController.isValid)
                InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left,
                    ref LeftController);
            if (!Hmd.isValid)
                InitializeInputDevice(InputDeviceCharacteristics.HeadMounted, ref Hmd);
        }

        private static void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics,
            ref InputDevice inputDevice)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);

            if (devices.Count > 0)
            {
                inputDevice = devices[0];
            }
        }
    }
}