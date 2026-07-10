using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _Project._Scripts
{
    public class CameraSwitcher : MonoBehaviour
    {
        public GameObject xrPrefab;
        public List<Transform> cameraPositions;
        public Button replayButton;
        public Button nextCameraButton;
        public List<GameObject> objectsToEnable;
        public List<GameObject> objectsToDisable;
        public InputActionProperty switchCamera;
        public InputActionProperty restartLevel;

        private GameObject _xr;
        private int _currentPositionIndex;

        private void Start()
        {
            replayButton.onClick.AddListener(MoveToFirstPosition);
            nextCameraButton.onClick.AddListener(MoveToNextPosition);
        }

        private void Update()
        {
            if (switchCamera.action.WasPressedThisFrame())
            {
                MoveToNextPosition();
            }

            if (restartLevel.action.WasPressedThisFrame())
            {
                LevelManager.LoadScene("MainMenu");
            }
        }

        private void MoveToFirstPosition()
        {
            if (cameraPositions.Count <= 0) return;

            if (_xr != null)
            {
                Destroy(_xr);
            }

            //_xr = Instantiate(xrPrefab, null, true); ---old option---
            xrPrefab.SetActive(true);
            _xr = xrPrefab;

            MoveXR(cameraPositions[0]);
            _currentPositionIndex = 0;

            foreach (var go in objectsToEnable)
            {
                go.SetActive(true);
            }

            foreach (var go in objectsToDisable)
            {
                go.SetActive(false);
            }
        }

        private void MoveToNextPosition()
        {
            if (cameraPositions.Count <= 0) return;
            _currentPositionIndex = (_currentPositionIndex + 1) % cameraPositions.Count;
            MoveXR(cameraPositions[_currentPositionIndex]);
        }

        private void MoveXR(Transform targetTransform)
        {
            if (_xr == null) return;

            _xr.transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y - 1.15f, targetTransform.position.z);
            _xr.transform.SetParent(targetTransform);
            _xr.transform.rotation = targetTransform.rotation;
        }
    }
}