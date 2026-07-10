using System.Collections;
using Simple_Bicycle_Physics.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace _Project._Scripts.Nick
{
    public class SpeedEventTrigger : MonoBehaviour
    {
        [SerializeField] private string activatorTag = "Player";
        public GameObject hitObject;
        public UnityEvent onTargetReached;

        public float delay;
        
        private bool hasBeenActivated;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(activatorTag)) return;
            if (hasBeenActivated) return;
            
            StartCoroutine(CalculateAndTriggerEvent(other.transform));
            hasBeenActivated = true;
        }

        private IEnumerator CalculateAndTriggerEvent(Transform player)
        {
            var distance = Vector3.Distance(player.transform.position, hitObject.transform.position);
            var speed = BicycleController.currentSpeed;
            
            while (true)
            {
                if (speed <= 0)
                {
                    yield return null;
                    continue;
                }

                var timeToReach = distance / (speed / 3.6f) - delay;

                Debug.Log($"Distance: {distance}, Speed: {speed}, Time to reach: {timeToReach}");

                yield return new WaitForSeconds(timeToReach);
                onTargetReached.Invoke();
                yield break;
            }
        }
    }
}