using System;
using UnityEngine;

namespace _Project._Scripts
{
    public class PlayerEventController : MonoBehaviour
    {
        public Collider stopTriggerZone;
        public static event Action<GameState> OnGameStateChange;
        public float requiredStopTime = 2f;

        private float SpeedKmH => GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        private float _stopDuration;
        private bool _isCrashed;

        private void OnCollisionEnter(Collision collision)
        {
            //if (!collision.gameObject.CompareTag("Obstacle")) return;

            Debug.Log("Collision with Door Detected: GameState Crashed");
            _isCrashed = true;
            OnGameStateChange?.Invoke(GameState.Crashed);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != stopTriggerZone) return;

            _stopDuration = 0f;
            Debug.Log("Player Entered Stop Trigger Zone");
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isCrashed) return;

            if (other == stopTriggerZone)
            {
                if (SpeedKmH < 0.1f)
                {
                    _stopDuration += Time.deltaTime;
                    if (_stopDuration >= requiredStopTime)
                    {
                        Debug.Log(
                            $"Player Stopped Successfully for {requiredStopTime} seconds: GameState StoppedSuccessfully");
                        OnGameStateChange?.Invoke(GameState.StoppedSuccessfully);
                        enabled = false;
                    }
                }
                else
                {
                    if (_stopDuration != 0f)
                    {
                        Debug.Log("Player Moved: Resetting Stop Duration");
                    }

                    _stopDuration = 0f;
                }
            }
        }
    }
}