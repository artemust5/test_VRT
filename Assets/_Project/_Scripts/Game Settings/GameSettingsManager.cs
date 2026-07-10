using System;
using _Project._Scripts.Game_Settings.@enum;
using _Project._Scripts.Game_Settings.SO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project._Scripts.Game_Settings
{
    public class GameSettingsManager : MonoBehaviour
    {
        [SerializeField] private GameSettingsConfig config;
        [SerializeField] private string sceneToLoad;

        private void Start()
        {
            if (config != null)
            {
                ApplyInitialSettings();
            }

            DontDestroyOnLoad(gameObject);
            // LoadScene();
        }

        private void ApplyInitialSettings()
        {
            SetResolution(config.resolution);
            SetTargetFPS(config.targetFPS);
            SetDisplayMode(config.displayMode);
        }

        private static void SetResolution(ScreenResolution resolution)
        {
            switch (resolution)
            {
                case ScreenResolution.R1920X1080:
                    Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
                    break;
                case ScreenResolution.R1280X720:
                    Screen.SetResolution(1280, 720, Screen.fullScreenMode);
                    break;
                case ScreenResolution.R2560X1440:
                    Screen.SetResolution(2560, 1440, Screen.fullScreenMode);
                    break;
                case ScreenResolution.R3840X2160:
                    Screen.SetResolution(3840, 2160, Screen.fullScreenMode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
        }

        private static void SetTargetFPS(FrameRate fps)
        {
            Application.targetFrameRate = (int)fps;
        }

        private static void SetDisplayMode(DisplayMode mode)
        {
            Screen.fullScreenMode = mode switch
            {
                DisplayMode.Fullscreen => FullScreenMode.ExclusiveFullScreen,
                DisplayMode.Windowed => FullScreenMode.Windowed,
                DisplayMode.Borderless => FullScreenMode.FullScreenWindow,
                _ => Screen.fullScreenMode
            };
        }

        private void LoadScene()
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}