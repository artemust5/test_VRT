using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project._Scripts.Nick
{
    public class VRControllerButtonFeedback : MonoBehaviour
    {
        [System.Serializable]
        public class ButtonFeedback
        {
            public InputActionReference buttonAction;
            public GameObject buttonImage;
        }

        public ButtonFeedback stickFeedback;
        public ButtonFeedback bButtonFeedback;
        public ButtonFeedback aButtonFeedback;


        private void OnEnable()
        {
            SetupButtonFeedback(stickFeedback);
            SetupButtonFeedback(bButtonFeedback);
            SetupButtonFeedback(aButtonFeedback);
        }

        private void SetupButtonFeedback(ButtonFeedback feedback)
        {
            if (feedback.buttonAction != null && feedback.buttonImage != null)
            {
                feedback.buttonAction.action.performed += _ => HighlightButton(feedback, true);
                feedback.buttonAction.action.canceled += _ => HighlightButton(feedback, false);
                feedback.buttonAction.action.Enable();
            }
        }

        private void HighlightButton(ButtonFeedback button, bool state)
        {
            if (button.buttonImage != null)
            {
                button.buttonImage.SetActive(state);
            }
        }
    }
}