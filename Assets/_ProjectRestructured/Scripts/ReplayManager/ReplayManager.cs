using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace _ProjectRestructured.Scripts.ReplayManager
{
    public enum ReplayMode
    {
        Idle,
        Recording,
        Playback
    }

    public class ReplayManager : MonoBehaviour
    {
        [Header("Replay Settings")] public ReplayMode mode = ReplayMode.Idle;
        public List<ReplayableObject> replayObjects = new();

        [Header("Playback Controls")] public float playbackSpeed = 1f;
        public bool isPlaying;

        private List<ReplayFrame> _frames = new();
        private float _recordingTime;
        private int _currentFrameIndex;
        private const float FixedDeltaTime = 0.02f;

        public static bool replaySaved;

        public float currentPlaybackTime { get; private set; }
        public float replayDuration => _frames is { Count: > 0 } ? _frames[^1].time : 0f;

        private void FixedUpdate()
        {
            switch (mode)
            {
                case ReplayMode.Recording:
                    RecordFrame();
                    break;
                case ReplayMode.Playback when isPlaying:
                    PlaybackFrame();
                    break;
                case ReplayMode.Playback:
                    break;
                case ReplayMode.Idle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RecordFrame()
        {
            var frame = new ReplayFrame
            {
                time = _recordingTime,
                objects = new List<ReplayFrameObject>()
            };

            foreach (var frameObj in replayObjects.Select(obj => new ReplayFrameObject
                     {
                         replayID = obj.ReplayID,
                         data = obj.CaptureReplayData()
                     }))
            {
                frame.objects.Add(frameObj);
            }

            _frames.Add(frame);
            _recordingTime += FixedDeltaTime;
        }

        private void PlaybackFrame()
        {
            if (_frames == null || _frames.Count == 0)
                return;

            currentPlaybackTime += FixedDeltaTime * playbackSpeed;

            var maxTime = _frames[^1].time;

            if (currentPlaybackTime >= maxTime)
            {
                currentPlaybackTime = 0;
                // currentPlaybackTime = maxTime;
                // _currentFrameIndex = _frames.Count - 1;
                // isPlaying = false;
                ApplyFrameData(_frames[_currentFrameIndex]);
                return;
            }

            while (_currentFrameIndex < _frames.Count - 1 &&
                   _frames[_currentFrameIndex + 1].time <= currentPlaybackTime)
            {
                _currentFrameIndex++;
            }

            ApplyFrameData(_frames[_currentFrameIndex]);
        }

        private void ApplyFrameData(ReplayFrame frame)
        {
            foreach (var frameObj in frame.objects)
            {
                var replayable = replayObjects.Find(x => x.ReplayID == frameObj.replayID);
                if (replayable)
                {
                    replayable.ApplyReplayData(frameObj.data);
                }
            }
        }

        #region Saving and Loading

        [Serializable]
        public class ReplayDataContainer
        {
            public List<ReplayFrame> frames;

            public ReplayDataContainer(List<ReplayFrame> frames)
            {
                this.frames = frames;
            }
        }

        public void SaveReplay()
        {
            if (_frames.Count == 0)
            {
                Debug.LogWarning("Nothing to save – no frames recorded.");
                return;
            }

            var container = new ReplayDataContainer(_frames);
            var json = JsonUtility.ToJson(container, true);

            var path = GetReplayFilePath();
            File.WriteAllText(path, json);
            Debug.Log("Replay saved to " + path);
            replaySaved = true;
        }

        public void LoadReplay(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("Replay file not found: " + filePath);
                return;
            }

            var json = File.ReadAllText(filePath);
            var container = JsonUtility.FromJson<ReplayDataContainer>(json);
            _frames = container.frames;

            currentPlaybackTime = 0f;
            _currentFrameIndex = 0;
            mode = ReplayMode.Playback;
        }

        // Create unique file name
        // private static string GetReplayFilePath()
        // {
        //     var directory = Application.persistentDataPath;
        //     if (!Directory.Exists(directory))
        //         Directory.CreateDirectory(directory);
        //
        //     var replayIndex = 1;
        //     string path;
        //     do
        //     {
        //         path = Path.Combine(directory, "replay_" + replayIndex.ToString("D2") + ".json");
        //         replayIndex++;
        //     } while (File.Exists(path));
        //
        //     return path;
        // }

        private static string GetReplayFilePath()
        {
            var directory = Application.persistentDataPath;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            return Path.Combine(directory, "replay.json");
        }

        #endregion

        #region UI Control Methods

        public void Play()
        {
            if (mode != ReplayMode.Playback)
            {
                Debug.LogWarning("Cannot play – not in playback mode.");
                return;
            }

            isPlaying = true;
        }

        public void Pause()
        {
            isPlaying = false;
        }

        public void SetPlaybackSpeed(float speed)
        {
            playbackSpeed = speed;
        }

        public void ScrubToTime(float time)
        {
            if (_frames.Count == 0)
                return;

            currentPlaybackTime = Mathf.Clamp(time, 0f, _frames[^1].time);
            _currentFrameIndex = 0;
            for (var i = 0; i < _frames.Count; i++)
            {
                if (_frames[i].time <= currentPlaybackTime)
                    _currentFrameIndex = i;
                else
                    break;
            }

            ApplyFrameData(_frames[_currentFrameIndex]);
        }

        #endregion

        #region Public API to Control Recording/Playback

        public void StartRecording()
        {
            mode = ReplayMode.Recording;
            _frames.Clear();
            _recordingTime = 0f;
        }

        public void StopRecording()
        {
            mode = ReplayMode.Idle;
        }
        
        public void StopRecordingWithDelay(float time)
        {
            StartCoroutine(StopRecordingCoroutine(time));
        }

        private IEnumerator StopRecordingCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            mode = ReplayMode.Idle;
        }

    public void StartPlayback() {
      if (_frames.Count == 0) return;

      mode = ReplayMode.Playback;
      currentPlaybackTime = 0f;
      _currentFrameIndex = 0;
      isPlaying = true;

      foreach (var rb in replayObjects.Select(replayable => replayable.GetComponent<Rigidbody>()).Where(rb => rb)) {
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.interpolation = RigidbodyInterpolation.None; // ВИМИКАЄМО ІНТЕРПОЛЯЦІЮ
      }
    }

    public void StopPlayback() {
      if (mode != ReplayMode.Playback) return;

      if (_frames.Count > 0) ApplyFrameData(_frames.Last());

      foreach (var rb in replayObjects.Select(replayable => replayable.GetComponent<Rigidbody>()).Where(rb => rb != null)) {
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // ПОВЕРТАЄМО ІНТЕРПОЛЯЦІЮ
      }

      currentPlaybackTime = 0f;
      _currentFrameIndex = 0;
      isPlaying = false;
      mode = ReplayMode.Idle;
    }

    #endregion
  }
}