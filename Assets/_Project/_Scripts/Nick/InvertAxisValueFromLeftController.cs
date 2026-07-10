using _ProjectRestructured.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Nick
{
    public class InvertAxisValueFromLeftController : MonoBehaviour
    {
        public TextMeshProUGUI textStatus;

        private Button button;
        private void Awake()
        {
            button = GetComponent<Button>();
            
            if (textStatus == null)
                Debug.LogError($"TextStatus not found on {gameObject.name}");
            
            button.onClick.AddListener(InvertValue);
            textStatus.text = DataManager.playerData.inverseAxisValue ? "On" : "Off";
        }
        
        private void OnDestroy()
        {
            if (button)
            {
                button.onClick.RemoveListener(InvertValue);
            }
        }

        private void InvertValue()
        {
            DataManager.playerData.inverseAxisValue = !DataManager.playerData.inverseAxisValue;
            textStatus.text = DataManager.playerData.inverseAxisValue ? "On" : "Off";
        }
    }
}