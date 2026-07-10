using UnityEngine;
using UnityEngine.Events;

namespace _Project._Scripts
{
    public class CollisionEventTrigger : MonoBehaviour
    {
        [SerializeField] private string activatorTag = "Player";
        [SerializeField] private bool activateOnlyOnce;
        [SerializeField] private UnityEvent onTriggerActivated;
        [SerializeField] private bool debugTrigger;

        private bool _hasBeenActivated;

        private void OnValidate()
        {
            if (!debugTrigger) return;

            ActivateTriggerLogic("Debug Trigger");
            debugTrigger = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(activatorTag)) return;
            if (activateOnlyOnce && _hasBeenActivated) return;

            ActivateTriggerLogic(other.gameObject.name);

            if (activateOnlyOnce) _hasBeenActivated = true;
        }

        private void ActivateTriggerLogic(string activatorName)
        {
            Debug.Log("Trigger activated by " + activatorName);

            onTriggerActivated?.Invoke();
        }
    }
}