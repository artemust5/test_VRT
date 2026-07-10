using UnityEngine;

namespace _ProjectRestructured.Scripts.ReplayManager
{
    /// Debug controller to test the ReplayManager via keyboard commands.
    /// 
    /// Key mappings (feel free to change them):
    ///  R     : Start Recording
    ///  T     : Stop Recording
    ///  S     : Save Replay
    ///  P     : Start Playback (resets the playback time)
    ///  Space : Toggle Play/Pause during playback
    ///  Left Arrow  : Scrub backward (decrease playback time)
    ///  Right Arrow : Scrub forward (increase playback time)
    ///  Up Arrow    : Increase playback speed
    ///  Down Arrow  : Decrease playback speed
    public class ReplayDebugController : MonoBehaviour
    {
        public _ProjectRestructured.Scripts.ReplayManager.ReplayManager replayManager;

        private float _debugPlaybackTime;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                replayManager.StartRecording();
                Debug.Log("Recording started.");
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                replayManager.StopRecording();
                Debug.Log("Recording stopped.");
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                replayManager.SaveReplay();
                Debug.Log("Replay saved.");
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                replayManager.StartPlayback();
                _debugPlaybackTime = 0f;
                Debug.Log("Playback started.");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (replayManager.isPlaying)
                {
                    replayManager.Pause();
                    Debug.Log("Playback paused.");
                }
                else
                {
                    replayManager.Play();
                    Debug.Log("Playback resumed.");
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _debugPlaybackTime -= 0.01f;
                if (_debugPlaybackTime < 0f)
                    _debugPlaybackTime = 0f;
                replayManager.ScrubToTime(_debugPlaybackTime);
                Debug.Log("Scrubbing backward to time: " + _debugPlaybackTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                _debugPlaybackTime += 0.01f;
                replayManager.ScrubToTime(_debugPlaybackTime);
                Debug.Log("Scrubbing forward to time: " + _debugPlaybackTime);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                var newSpeed = replayManager.playbackSpeed + 0.1f;
                replayManager.SetPlaybackSpeed(newSpeed);
                Debug.Log("Increased playback speed to: " + newSpeed);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var newSpeed = Mathf.Max(0.1f, replayManager.playbackSpeed - 0.1f);
                replayManager.SetPlaybackSpeed(newSpeed);
                Debug.Log("Decreased playback speed to: " + newSpeed);
            }
        }
    }
}