using System;
using System.Collections;
using _ProjectRestructured.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project._Scripts.Nick
{
    public class XRControllerReset : MonoBehaviour
    {
        public InputActionReference resetAction;
        public static Action onXrResetPerformed;

        public Transform head;
        public Transform origin;
        public GameObject cameraXR;
        public GameObject cameraPC;
        
        private bool isPcView;

        private void Start()
        {
            StartCoroutine(RecenterAfterDelay(1f));

            if (resetAction != null && resetAction.action != null)
            {
                resetAction.action.performed += OnResetPerformed;
                resetAction.action.Enable();
            }
            else
            {
                Debug.LogError("Reset Action is not set or its action is null!");
            }
        }

        private void OnDestroy()
        {
            if (resetAction == null || resetAction.action == null) return;
            resetAction.action.performed -= OnResetPerformed;
            resetAction.action.Disable();
        }

        private void OnResetPerformed(InputAction.CallbackContext context)
        {
            RecenterFewTimes();
        }

        private IEnumerator RecenterAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            onXrResetPerformed?.Invoke();
            onXrResetPerformed?.Invoke();
            onXrResetPerformed?.Invoke();
            onXrResetPerformed?.Invoke();
        }

        public void RecenterFewTimes()
        {
            // Recenter();
            // Recenter();
            // Recenter();
            // Recenter();
            onXrResetPerformed?.Invoke();
            onXrResetPerformed?.Invoke();
            onXrResetPerformed?.Invoke();
            onXrResetPerformed?.Invoke();
        }

        private void Recenter()
        {
            var target = VehicleManager.Instance.Current.xrAnchor;

            var headOffset = head.position - origin.position;
            origin.position = target.position - headOffset;

            var forwardTarget = target.forward;
            var forwardHead = head.forward;

            forwardTarget.y = 0;
            forwardHead.y = 0;

            if (forwardTarget != Vector3.zero && forwardHead != Vector3.zero)
            {
                var angle = Vector3.SignedAngle(forwardHead, forwardTarget, Vector3.up);
                origin.RotateAround(head.position, Vector3.up, angle);
            }

            onXrResetPerformed?.Invoke();
        }
        
        public void ChangeTarget()
        {
            isPcView = !isPcView;
            cameraXR.SetActive(!isPcView);
            cameraPC.SetActive(isPcView);
        }
    }
}
