using System.Collections.Generic;
using _ProjectRestructured.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project._Scripts
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField] private RTC_TrafficLight trafficLight;

        [Header("Menu")] 
        [SerializeField] private GameObject mainMenu;

        [Header("Cameras")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private List<Camera> cameraList;

        [Header("Buttons")] 
        [SerializeField] private Button toggleMainCameraButton;
        [SerializeField] private Button cycleCamerasButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button restartButton;

        private bool _isMenuActive;
        private int _currentCameraIndex;

        private void Start()
        {
            if (mainMenu != null)
            {
                mainMenu.SetActive(false);
                _isMenuActive = false;
            }
            else
            {
                Debug.LogError("MainMenu GameObject not found. Please make sure it is tagged correctly.");
            }

            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(true);
            }

            foreach (var cam in cameraList)
            {
                cam.gameObject.SetActive(false);
            }

            SetupButtonListenersUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMenu();
            }
        }

        private void SetupButtonListenersUI()
        {
            AddButtonListener(toggleMainCameraButton, ToggleMainCamera);
            AddButtonListener(cycleCamerasButton, CycleThroughCameras);
            AddButtonListener(mainMenuButton, LoadMainMenu);
            AddButtonListener(restartButton, RestartLevel);
        }

        private void ToggleMenu()
        {
            _isMenuActive = !_isMenuActive;
            mainMenu.SetActive(_isMenuActive);

            switch (_isMenuActive)
            {
                case true:
                    //PauseManager.PauseGame();
                    break;
                default:
                    //PauseManager.ResumeGame();
                    break;
            }
        }

        private static void AddButtonListener(Button button, UnityAction action)
        {
            if (button != null)
            {
                button.onClick.AddListener(action);
            }
        }

        public void LoadLevelAsync(string sceneName)
        {
            LevelManager.Instance.LoadScene(sceneName, "CrossFade");
        }

        public void RestartLevelAsync()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            LevelManager.Instance.LoadScene(currentSceneName, "CrossFade");
        }

        public void LoadLevel(string sceneName)
        {
            LevelManager.LoadScene(sceneName);
        }

        public void RestartLevel()
        {
            var currentSceneName = SceneManager.GetActiveScene().name;
            LevelManager.LoadScene(currentSceneName);
        }

        private void ToggleMainCamera()
        {
            mainCamera.gameObject.SetActive(false);

            if (cameraList.Count <= 0) return;
            cameraList[0].gameObject.SetActive(true);
            _currentCameraIndex = 0;
        }

        private void CycleThroughCameras()
        {
            if (cameraList.Count <= 0) return;

            cameraList[_currentCameraIndex].gameObject.SetActive(false);
            _currentCameraIndex = (_currentCameraIndex + 1) % cameraList.Count;
            cameraList[_currentCameraIndex].gameObject.SetActive(true);
        }

        public void SetTrafficLightState(string state)
        {
            switch (state)
            {
                case "Red":
                    trafficLight.SetRedLight();
                    break;
                case "Green":
                    trafficLight.SetGreenLight();
                    break;
                default:
                    Debug.LogError("Wrong state");
                    break;
            }
        }
        
        private static void LoadMainMenu()
        {
            LevelManager.LoadScene("MainMenu");
        }
    }
}