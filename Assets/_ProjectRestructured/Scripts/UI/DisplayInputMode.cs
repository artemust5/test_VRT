using System;
using _ProjectRestructured.Scripts.Data;
using UnityEngine;

namespace _ProjectRestructured.Scripts.UI
{
    public class DisplayInputMode : MonoBehaviour
    {
        public GameObject vrControllers;
        public GameObject realBicycle;
        public GameObject eScooter;

        private void Update()
        {
            switch (DataManager.playerData.inputMode)
            {
                case InputMode.RealBicycle:
                    realBicycle.SetActive(true);
                    vrControllers.SetActive(false);
                    eScooter.SetActive(false);
                    break;
                case InputMode.VRControllers:
                    realBicycle.SetActive(false);
                    vrControllers.SetActive(true);
                    eScooter.SetActive(false);
                    break;
                case InputMode.Keyboard:
                    realBicycle.SetActive(false);
                    vrControllers.SetActive(false);
                    eScooter.SetActive(false);
                    break;
                case InputMode.EScooter:
                    realBicycle.SetActive(false);
                    vrControllers.SetActive(false);
                    eScooter.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
