using UnityEngine;

namespace _ProjectRestructured.Scripts.Data
{
    public class SetInputMode : MonoBehaviour
    {
        public void SetMode(int value)
        {
            DataManager.playerData.inputMode = value switch
            {
                0 => InputMode.RealBicycle,
                1 => InputMode.VRControllers,
                _ => DataManager.playerData.inputMode
            };
        }
    }
}
