using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _ProjectRestructured.Scripts.UI
{
    public class ToggleSwitch : MonoBehaviour
    {
        [SerializeField] private bool isOn;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private GameObject targetObject;

        private void Start()
        {
            button.onClick.AddListener(ToggleState);
            UpdateState();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(ToggleState);
        }

        private void ToggleState()
        {
            isOn = !isOn;
            UpdateState();
        }

        private void UpdateState()
        {
            if (stateText != null)
            {
                stateText.text = isOn ? "On" : "Off";
            }

            if (targetObject != null)
            {
                targetObject.SetActive(isOn);
            }
        }
    }
}