using System;
using System.Collections;
using _ProjectRestructured.Scripts.Gameplay;
using UltimateReplay;
using UltimateReplay.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts
{
    public class HazardController : MonoBehaviour
    {
        public string replayFile = "MyReplay.replay";
        public Transform doorTransform;
        public float openAngle = 90f;
        public float openSpeed = 2f;
        public GameObject finishLevelUI;
        public GameObject resultUI;
        public Text stateText;

        private bool _isOpen;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;

        private ReplayRecordOperation _recordOp = null;
        private ReplayStorage _storage;
        private ReplayPlaybackOperation _playbackOp;

        private bool _isRecordingStarted;
        private bool _isRecordingFinished;

        

        private void OnEnable()
        {
            PlayerEventController.OnGameStateChange += HandleGameStateChange;
        }

        private void OnDisable()
        {
            PlayerEventController.OnGameStateChange -= HandleGameStateChange;
        }

        private void Start()
        {
            replayFile = Application.persistentDataPath + "/" + replayFile + ReplayControls.uniqCodeForReplayName;
            InitializeDoorOpening();
        }

        public void ToggleDoor()
        {
            StopAllCoroutines();
            StartCoroutine(OpenCloseDoor());
        }

        public void StartRecording()
        {
            if (_isRecordingStarted) return;

            _storage = ReplayFileStorage.FromFile(replayFile);
            _recordOp = ReplayManager.BeginRecording(_storage);
            _isRecordingStarted = true;
            Debug.Log("Started recording");
        }

        public void InitiateStopRecording(float stopRecordingDelay)
        {
            if (_isRecordingFinished) return;

            StartCoroutine(StopRecordingAfterDelay(stopRecordingDelay));
        }

        private void InitializeDoorOpening()
        {
            _closedRotation = doorTransform.rotation;
            var eulerAngles = doorTransform.eulerAngles;
            _openRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + openAngle, eulerAngles.z);
        }

        private IEnumerator OpenCloseDoor()
        {
            var startTime = Time.time;
            var startRotation = doorTransform.rotation;
            var endRotation = _isOpen ? _closedRotation : _openRotation;

            while (Time.time - startTime < 1f / openSpeed)
            {
                doorTransform.rotation =
                    Quaternion.Lerp(startRotation, endRotation, (Time.time - startTime) * openSpeed);
                yield return null;
            }

            doorTransform.rotation = endRotation;
            _isOpen = !_isOpen;
        }

        private IEnumerator StopRecordingAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_recordOp == null) yield break;
            _recordOp.StopRecording();
            _isRecordingStarted = false;
            _isRecordingFinished = true;
            finishLevelUI.SetActive(true);
            //PauseManager.PauseGame();
            Debug.Log("Ending recording");
        }

        private void Replay()
        {
            _playbackOp = ReplayManager.BeginPlayback(_storage);
        }

        private void HandleGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Crashed:
                    InitiateStopRecording(1f);
                    stateText.text = "Failed";
                    stateText.color = Color.red;
                    resultUI.SetActive(false);
                    break;
                case GameState.StoppedSuccessfully:
                    InitiateStopRecording(1f);
                    stateText.text = "SUCCESS!";
                    stateText.color = Color.green;
                    resultUI.SetActive(true);
                    break;
                case GameState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }
        }
    }
}