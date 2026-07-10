using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _ProjectRestructured.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts
{
    public class PreStartLevel : MonoBehaviour
    {
        [SerializeField] private Button readyButton;
        [SerializeField] private Text countdownText;
        [SerializeField] private GameObject preStartUI;

        [SerializeField] private GameObject car;
        [SerializeField] private float desiredHeight;
        [SerializeField] private List<RTC_CarController> objectsWithRtc;

        private bool _isPreStartUINotNull;

        private void Awake()
        {
            if (preStartUI != null)
            {
                preStartUI.SetActive(true);
            }

            readyButton.onClick.AddListener(OnReadyButtonClicked);

            SetTransform();
        }

        private void Start()
        {
            _isPreStartUINotNull = preStartUI != null;
            //PauseManager.PauseGame();
            // EnableAllRtcCarControllers();
        }

        private void OnReadyButtonClicked()
        {
            StartCoroutine(CountdownCoroutine());
            readyButton.gameObject.SetActive(false);
        }

        private void SetTransform()
        {
            var currentPosition = car.transform.position;
            car.transform.position = new Vector3(currentPosition.x, desiredHeight, currentPosition.z);
        }

        private void EnableAllRtcCarControllers()
        {
            // Iterate through each object in the list
            foreach (var rtcCarController in objectsWithRtc.Select(obj => obj.GetComponent<RTC_CarController>())
                         .Where(rtcCarController => rtcCarController != null))
            {
                rtcCarController.enabled = true;
            }
        }

        private IEnumerator CountdownCoroutine()
        {
            var countdownTime = 3;
            countdownText.gameObject.SetActive(true);

            while (countdownTime > 0)
            {
                countdownText.text = countdownTime.ToString();
                yield return new WaitForSecondsRealtime(1);
                countdownTime--;
            }

            countdownText.text = "Go!";
            yield return new WaitForSecondsRealtime(1);

            if (_isPreStartUINotNull)
                preStartUI.SetActive(false);

            //PauseManager.ResumeGame();
        }
    }
}