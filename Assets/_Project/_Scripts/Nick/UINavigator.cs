using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

namespace _Project._Scripts.Nick
{
    public class UINavigator : MonoBehaviour
    {
        public List<Button> buttons;
        public float joystickThreshold = 0.5f;
        public float moveDelay = 0.2f;

        public InputActionReference navigateAction;
        public InputActionReference selectAction;

        public Action<int> OnButtonSelected;

        private int currentIndex;
        private bool joystickMoved;
        private float lastMoveTime;

        private void Start()
        {
            CollectButtons();
        }

        private void OnEnable()
        {
            navigateAction.action.performed += OnNavigate;
            selectAction.action.performed += OnSelect;
            navigateAction.action.Enable();
            selectAction.action.Enable();

            HighlightFirstButtonInMenu();
        }

        private void OnDisable()
        {
            navigateAction.action.performed -= OnNavigate;
            selectAction.action.performed -= OnSelect;
            navigateAction.action.Disable();
            selectAction.action.Disable();
        }
        private void CollectButtons()
        {
            if (buttons.Count > 0)
            {
                HighlightFirstButtonInMenu();
            }
            else
            {
                Debug.LogWarning("No active buttons found in the scene.");
            }
        }

        private void OnNavigate(InputAction.CallbackContext context)
        {
            if (buttons.Count == 0) return;

            var input = context.ReadValue<Vector2>();
            if (input.x > joystickThreshold && !joystickMoved && Time.time - lastMoveTime > moveDelay)
            {
                SelectNextButton(1); 
                joystickMoved = true;
                lastMoveTime = Time.time;
            }
            else if (input.x < -joystickThreshold && !joystickMoved && Time.time - lastMoveTime > moveDelay)
            {
                SelectNextButton(-1); 
                joystickMoved = true;
                lastMoveTime = Time.time;
            }
            else if (Mathf.Abs(input.x) < joystickThreshold)
            {
                joystickMoved = false;
            }
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            PressSelectedButton();
        }

        public void SelectNextButton(int direction)
        {
            if (buttons.Count == 0) return;

            buttons[currentIndex].OnDeselect(null);
            currentIndex = (currentIndex + direction + buttons.Count) % buttons.Count;
            buttons[currentIndex].OnSelect(null);
            OnButtonSelected?.Invoke(currentIndex);
        }

        public void PressSelectedButton()
        {
            if (buttons.Count > 0 && currentIndex >= 0 && currentIndex < buttons.Count)
            {
                buttons[currentIndex].onClick.Invoke();
            }
        }

        private void HighlightFirstButtonInMenu()
        {
            currentIndex = 0;
            buttons[currentIndex].OnSelect(null);
        }
    }
}