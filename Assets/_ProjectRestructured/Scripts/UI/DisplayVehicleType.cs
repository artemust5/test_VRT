using System;
using _ProjectRestructured.Scripts.Data;
using UnityEngine;

namespace _ProjectRestructured.Scripts.UI
{
    public class DisplayVehicleType : MonoBehaviour
    {
        public GameObject bicycle;
        public GameObject eScooter;

        private void Update()
        {
            switch (DataManager.playerData.vehicleType)
            {
                case VehicleType.Bicycle:
                    bicycle.SetActive(true);
                    eScooter.SetActive(false);
                    break;
                case VehicleType.EScooter:
                    bicycle.SetActive(false);
                    eScooter.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
