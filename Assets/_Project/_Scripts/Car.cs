using _ProjectRestructured.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace _Project._Scripts
{
    public class Car : MonoBehaviour
    {
        [SerializeField] private UnityEvent onCollision;
        [SerializeField] private GameObject endGameUI;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            ActivateTriggerLogic(other.gameObject.name);
        }

        private void ActivateTriggerLogic(string activatorName)
        {
            Debug.Log($"Trigger activated by {activatorName}");
            EndGame();
            onCollision?.Invoke();
        }

        private void EndGame()
        {
            //PauseManager.PauseGame();

            if (endGameUI != null)
            {
                endGameUI.SetActive(true);
            }
            else
            {
                Debug.LogError("EndGame object reference not set in the Inspector.");
            }
        }
    }
}