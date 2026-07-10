using System.Collections;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Gameplay
{
    public class PauseManager : MonoBehaviour
    {
        public RTC_CarController[] rtcCarControllers;

        private const float MaxVolume = 1f;
        private const float MediumVolume = 0.3f;
        private const float FadeDuration = 1.5f;

        private void Start()
        {
            AudioListener.volume = 0f;
        }

        public void Initialize()
        {
            StartCoroutine(FadeVolume(MediumVolume));
            foreach (var controller in rtcCarControllers)
            {
                if (controller != null)
                {
                    controller.enabled = false;
                }
            }
        }

        public void PauseGame()
        {
            StartCoroutine(FadeVolume(0f));

            foreach (var controller in rtcCarControllers)
            {
                if (controller != null)
                {
                    controller.enabled = false;
                }
            }
        }

        public void ResumeGame()
        {
            StartCoroutine(FadeVolume(MaxVolume));

            foreach (var controller in rtcCarControllers)
            {
                if (controller != null)
                {
                    controller.enabled = true;
                }
            }
        }

        private IEnumerator FadeVolume(float targetVolume)
        {
            var startVolume = AudioListener.volume;
            var elapsedTime = 0f;
    
            while (elapsedTime < FadeDuration)
            {
                elapsedTime += Time.deltaTime;
                AudioListener.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / FadeDuration);
                yield return null;
            }
        }
    }
}