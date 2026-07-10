using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _ProjectRestructured.Scripts.ReplayManager
{
    public class ReplayUI : MonoBehaviour
    {
        public ReplayManager replayManager;
        public Slider scrubSlider;
        public Button playPauseButton;
        //public Button stopButton;

        public Button switchCameraButton;
        public Transform[] camerasTransform;
        public GameObject replayXROrigin;
        public Transform headTransform;
        public Transform hudTransform;
        public GameObject[] activateObjects;
        public GameObject[] deactivateObjects;

        private int currentCameraIndex = -1;
        private bool cameraSwitcherInitialized;
        private Text playPauseButtonText;

        private void Start()
        {
            if (scrubSlider != null)
                scrubSlider.onValueChanged.AddListener(OnSliderValueChanged);

            if (playPauseButton != null)
            {
                playPauseButton.onClick.AddListener(OnPlayPauseClicked);
                playPauseButtonText = playPauseButton.GetComponentInChildren<Text>();
                UpdatePlayPauseButtonText();
            }
            //if (stopButton != null)
            //stopButton.onClick.AddListener(OnStopClicked);
        }

        private void InitializeCameraSwitcher()
        {
            foreach (var deactivateObject in deactivateObjects)
            {
                deactivateObject.SetActive(false);
            }
            foreach (var activateObject in activateObjects)
            {
                activateObject.SetActive(true);
            }
            
            switchCameraButton.onClick.AddListener(() =>
            {
                var nextCameraTransform = GetNextCameraTransform();
                replayXROrigin.transform.SetParent(nextCameraTransform);
                hudTransform.SetParent(nextCameraTransform);
                hudTransform.localRotation = Quaternion.identity;
                hudTransform.localPosition = new Vector3(0,0,0.6f);
                
                Recenter(nextCameraTransform);
                Recenter(nextCameraTransform);
                Recenter(nextCameraTransform);
                Recenter(nextCameraTransform);
            });
            StartCoroutine(RecenterAfterDelay());
        }
        private IEnumerator RecenterAfterDelay()
        {
            yield return null;
            switchCameraButton.onClick.Invoke();
        }
        private void Recenter(Transform target)
        {
            var headPosition = headTransform.position;
            var offset = headPosition - replayXROrigin.transform.position;
            replayXROrigin.transform.position = target.position - offset;
            
            var targetForward = target.forward;
            targetForward.y = 0;
            var cameraForward = headTransform.forward;
            cameraForward.y = 0;
            
            var angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);
            
            replayXROrigin.transform.RotateAround(headPosition, Vector3.up, angle);
        }
        private Transform GetNextCameraTransform()
        {
            currentCameraIndex = (currentCameraIndex + 1) % camerasTransform.Length;
            return camerasTransform[currentCameraIndex];
        }

        private void OnSliderValueChanged(float value)
        {
            if (replayManager == null) return;

            replayManager.ScrubToTime(value);
            Debug.Log("Scrubbed to time: " + value);
        }

        private void OnPlayPauseClicked()
        {
            if (replayManager == null) return;

            if (replayManager.isPlaying)
                replayManager.Pause();
            else if (replayManager.mode == ReplayMode.Playback)
                replayManager.Play();

            UpdatePlayPauseButtonText();
        }

        private void OnStopClicked()
        {
            if (replayManager == null) return;

            replayManager.StopPlayback();
            Debug.Log("Playback stopped.");
            UpdatePlayPauseButtonText();
        }

        private void Update()
        {
            if (replayManager && replayManager.mode == ReplayMode.Playback)
            {
                if (!cameraSwitcherInitialized)
                {
                    InitializeCameraSwitcher();
                    cameraSwitcherInitialized = true;
                }
                scrubSlider.maxValue = replayManager.replayDuration;
                scrubSlider.value = replayManager.currentPlaybackTime;
            }

            UpdatePlayPauseButtonText();
        }

        private void UpdatePlayPauseButtonText()
        {
            if (playPauseButtonText && replayManager)
                playPauseButtonText.text = replayManager.isPlaying ? "Pause" : "Play";
        }
    }
}