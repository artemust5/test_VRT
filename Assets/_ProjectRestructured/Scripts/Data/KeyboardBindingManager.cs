using _Project._Scripts.Nick;
using _ProjectRestructured.Scripts.Gameplay;
using _ProjectRestructured.Scripts.Player;
using _ProjectRestructured.Scripts.UI;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Data
{
    [System.Serializable]
    public class KeyboardBindings
    {
        [Header("XR")]
        public KeyCode inputModeKey;
        public KeyCode resetViewKey;
        public KeyCode resetSteeringKey;
        public KeyCode changeRecenterPositionKey;

        [Header("UI Navigation")]
        public KeyCode navigatePrevKey;
        public KeyCode navigateNextKey;
        public KeyCode selectKey;

        [Header("Scene Loading")]
        public KeyCode restartSceneKey;
        public KeyCode nextSceneKey;
        public KeyCode menuSceneKey;

        [Header("Menu")]
        public KeyCode openSettingMenuKey;

        [Header("Other")]
        public KeyCode vehicleTypeKey;
        public KeyCode blackScreenKey;
        public KeyCode phoneRingingKey;
        public KeyCode drunkDelayKey;
        public KeyCode replayKey;
    }

    public class KeyboardBindingManager : MonoBehaviour
    {
        [Header("BLE Numpad Bindings")]
        public KeyboardBindings bleNumpadBindings;

        [Header("Standard Keyboard Bindings")]
        public KeyboardBindings keyboardBindings;

        [Header("Managers Link")]
        public XRControllerReset xrControllerReset;
        public InputManager inputManager;
        public SceneHandler sceneHandler;
        public MenuScreensController menuScreenController;
        public PauseManager pauseManager;
        public GameObject bossRingingScreen;
        public ResultGraphicUI resultGraphicUI;
        public ReplayManager.ReplayManager replayManager;

        private bool isSceneLoading;
        private bool blackScreenEnabled;

        private void Update()
        {
            // XR
            if (GetKeyDown(bleNumpadBindings.inputModeKey, keyboardBindings.inputModeKey))
                DataManager.ChangeInputMode();

            if (GetKeyDown(bleNumpadBindings.resetViewKey, keyboardBindings.resetViewKey))
                xrControllerReset.RecenterFewTimes();

            if (GetKeyDown(bleNumpadBindings.resetSteeringKey, keyboardBindings.resetSteeringKey))
                inputManager.ResetXRControllerCenteredPoint();

            if (GetKeyDown(bleNumpadBindings.changeRecenterPositionKey, keyboardBindings.changeRecenterPositionKey))
            {
                xrControllerReset.ChangeTarget();
            }

            // UI Navigation
            if (GetKeyDown(bleNumpadBindings.navigatePrevKey, keyboardBindings.navigatePrevKey))
                FindObjectOfType<UINavigator>()?.SelectNextButton(-1);

            if (GetKeyDown(bleNumpadBindings.navigateNextKey, keyboardBindings.navigateNextKey))
                FindObjectOfType<UINavigator>()?.SelectNextButton(1);

            if (GetKeyDown(bleNumpadBindings.selectKey, keyboardBindings.selectKey))
                FindObjectOfType<UINavigator>()?.PressSelectedButton();

            // Scene Loading
            if (GetKeyDown(bleNumpadBindings.restartSceneKey, keyboardBindings.restartSceneKey) && !isSceneLoading)
            {
                sceneHandler.RestartScene();
                isSceneLoading = true;
            }

            if (GetKeyDown(bleNumpadBindings.nextSceneKey, keyboardBindings.nextSceneKey) && !isSceneLoading)
            {
                sceneHandler.LoadNextScene();
                isSceneLoading = true;
            }

            if (GetKeyDown(bleNumpadBindings.menuSceneKey, keyboardBindings.menuSceneKey) && !isSceneLoading)
            {
                sceneHandler.LoadMenuScene();
                isSceneLoading = true;
            }

            // Menu
            if (GetKeyDown(bleNumpadBindings.openSettingMenuKey, keyboardBindings.openSettingMenuKey))
                menuScreenController.HandleOpeningSettingsMenuPanel();

            // Other
            if (GetKeyDown(bleNumpadBindings.vehicleTypeKey, keyboardBindings.vehicleTypeKey))
                            VehicleManager.Instance.SwitchToNext();
            
            if (GetKeyDown(bleNumpadBindings.blackScreenKey, keyboardBindings.blackScreenKey))
            {
                if (blackScreenEnabled)
                {
                    sceneHandler.DisableBlackScreen();
                    pauseManager.PauseGame();
                }
                else
                {
                    sceneHandler.EnableBlackScreen();
                    pauseManager.ResumeGame();
                }
                blackScreenEnabled = !blackScreenEnabled;
            }

            if (GetKeyDown(bleNumpadBindings.phoneRingingKey, keyboardBindings.phoneRingingKey))
                bossRingingScreen.SetActive(!bossRingingScreen.activeSelf);

            if (GetKeyDown(bleNumpadBindings.drunkDelayKey, keyboardBindings.drunkDelayKey))
                resultGraphicUI.SwitchDrunkDelay();

            if (GetKeyDown(bleNumpadBindings.replayKey, keyboardBindings.replayKey) &&
                ReplayManager.ReplayManager.replaySaved)
            {
                menuScreenController.DeactivateScreens();
                replayManager.StartPlayback();
            }
        }

        private bool GetKeyDown(KeyCode a, KeyCode b)
        {
            return Input.GetKeyDown(a) || Input.GetKeyDown(b);
        }
    }
}
