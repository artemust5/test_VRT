using UnityEngine;

namespace _Project._Scripts
{
    public class MouseLook : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity = 100f;
        [Range(0, 90)] [SerializeField] private int minRotationYAngle;
        [Range(0, 90)] [SerializeField] private int maxRotationYAngle;

        private float _xRotation;
        private float _yRotation;

        private void Start()
        {
            // Cursor.lockState = CursorLockMode.Locked;

            var currentRotation = transform.localRotation.eulerAngles;
            _xRotation = currentRotation.x;
            _yRotation = currentRotation.y;

            if (_xRotation > 180)
            {
                _xRotation -= 360;
            }
        }

        private void Update()
        {
            var mouseX = Input.GetAxis("Mouse X") * (mouseSensitivity * 100) * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * (mouseSensitivity * 100) * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -minRotationYAngle, maxRotationYAngle);

            _yRotation += mouseX;

            transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        }
    }
}