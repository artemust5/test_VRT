using _ProjectRestructured.Scripts.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project._Scripts.Nick
{
    public class ChangeInputModeOnController : MonoBehaviour
    {
        public InputActionReference changeAction;
        
        private void Start()
        {
            if (changeAction != null && changeAction.action != null)
            {
                changeAction.action.performed += OnResetPerformed;
                changeAction.action.Enable();
            }
            else
            {
                Debug.LogError("Reset Action is not set or its action is null!");
            }
        }
        private void OnDestroy()
        {
            if (changeAction == null || changeAction.action == null) return;
            changeAction.action.performed -= OnResetPerformed;
            changeAction.action.Disable();
        }
        
        private static void OnResetPerformed(InputAction.CallbackContext context)
        {
            DataManager.ChangeInputMode();
        }
    }
}
