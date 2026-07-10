using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _ProjectRestructured.Scripts.UI
{
    public class MenuScreensController : MonoBehaviour
    {
        public GameObject startScreen;
        public GameObject finishScreenSuccessful;
        public GameObject finishScreenFailed;
        public GameObject crashScreen;
        public GameObject boundaryScreen;
        public GameObject completeScreen;
        public GameObject settingsMenu;
        public GameObject settingsLanguageMenu;
        public GameObject[] allScreens;

        public InputActionReference openSettingsMenuAction;
        private GameObject activeScreenBeforeSettings;

        private void OnEnable()
        {
            if (openSettingsMenuAction != null && openSettingsMenuAction.action != null)
            {
                openSettingsMenuAction.action.performed += OnActivationActionPerformed;
                openSettingsMenuAction.action.Enable();
            }
            else
            {
                Debug.LogError("Activation Action is not set or its action is null!");
            }
        }

        private void OnDisable()
        {
            if (openSettingsMenuAction == null || openSettingsMenuAction.action == null) return;
            openSettingsMenuAction.action.performed -= OnActivationActionPerformed;
            openSettingsMenuAction.action.Disable();
        }

        private void OnActivationActionPerformed(InputAction.CallbackContext context)
        {
            HandleOpeningSettingsMenuPanel();
        }

        public void HandleOpeningSettingsMenuPanel()
        {
            if (settingsMenu.activeSelf)
                CloseSettingsMenu();
            else
                OpenSettingsMenuPanel();
        }

        private void OpenSettingsMenuPanel()
        {
            activeScreenBeforeSettings = allScreens.FirstOrDefault(menu => menu.activeSelf);
            activeScreenBeforeSettings?.SetActive(false);
            settingsLanguageMenu.SetActive(false);
            settingsMenu.SetActive(true);
        }

        public void CloseSettingsMenu()
        {
            settingsMenu.SetActive(false);
            settingsLanguageMenu.SetActive(false);

            if (activeScreenBeforeSettings == null) return;
            activeScreenBeforeSettings.SetActive(true);
            activeScreenBeforeSettings = null;
        }

        public void ActivateStartScreen()
        {
            DeactivateScreens();
            startScreen.SetActive(true);
        }
        public void CloseStartScreen()
        {
            DeactivateScreens();
        }
        public void ActivateBoundaryScreen()
        {
            DeactivateScreens();
            boundaryScreen.SetActive(true);
        }
        
        public void ActivateCrashScreen()
        {
            DeactivateScreens();
            crashScreen.SetActive(true);
        }
        
        public void ActivateCompleteScreen()
        {
            DeactivateScreens();
            completeScreen.SetActive(true);
        }
        public void ActivateFinishScreenSuccessful()
        {
            DeactivateScreens();
            finishScreenSuccessful.SetActive(true);
        }
        public void ActivateFinishScreenFailed()
        {
            DeactivateScreens();
            finishScreenFailed.SetActive(true);
        }

        public void DeactivateScreens()
        {
            activeScreenBeforeSettings = null;
            foreach (var screen in allScreens)
            {
                screen.SetActive(false);
            }
        }
    }
}