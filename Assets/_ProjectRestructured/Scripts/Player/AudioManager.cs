using Simple_Bicycle_Physics.Scripts;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Player
{
    public class AudioManager : MonoBehaviour
    {
        public AudioSource pedalsAudioSource;
        public AudioSource brakeAudioSource;
        public float minPitch = 0.8f, maxPitch = 1.5f, speedMultiplier = 0.05f;

        private void Start()
        {
            pedalsAudioSource.volume = 0f;
            brakeAudioSource.volume = 0f;
        }

        private void Update()
        {
            if (BicycleController.IsBraking && BicycleController.currentSpeed > 0.5f)
            {
                var normalizedSpeed = Mathf.Pow(BicycleController.currentSpeed * speedMultiplier, 2);
                brakeAudioSource.volume = 0.5f * normalizedSpeed;
                return;
            }
            brakeAudioSource.volume = 0f;
                
            if (BicycleController.currentSpeed > 0.5f)
            {
                var normalizedSpeed = Mathf.Pow(BicycleController.currentSpeed * speedMultiplier, 2);
                pedalsAudioSource.volume = normalizedSpeed;
                pedalsAudioSource.pitch = minPitch + (maxPitch - minPitch) * normalizedSpeed;
            }
            else
            {
                pedalsAudioSource.volume = 0f;
                pedalsAudioSource.pitch = minPitch;
            }
        }
    }
}
