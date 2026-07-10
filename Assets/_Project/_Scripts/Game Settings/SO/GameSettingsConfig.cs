using _Project._Scripts.Game_Settings.@enum;
using UnityEngine;

namespace _Project._Scripts.Game_Settings.SO
{
    [CreateAssetMenu(fileName = "GameSettingsConfig", menuName = "GameSettings/Config", order = 1)]
    public class GameSettingsConfig : ScriptableObject
    {
        public ScreenResolution resolution = ScreenResolution.R1920X1080;
        public DisplayMode displayMode = DisplayMode.Fullscreen;
        public FrameRate targetFPS = FrameRate.FPS60;
    }
}