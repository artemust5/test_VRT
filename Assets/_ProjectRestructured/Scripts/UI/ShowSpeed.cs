using _ProjectRestructured.Scripts.Player;
using Simple_Bicycle_Physics.Scripts;
using UnityEngine;

namespace _ProjectRestructured.Scripts.UI
{
    public class ShowSpeed : MonoBehaviour
    {

        private void Update()
        {
            VehicleManager.Instance.Current.speedText.text = $"{BicycleController.currentSpeed:0.0}";
        }
    }
}
