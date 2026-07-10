using System.Collections;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Gameplay.Hazard
{
    public class OpenDoor : MonoBehaviour
    {
        [SerializeField] private float targetAngle;
        [SerializeField] private float duration;

        public void Open()
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor());
        }

        private IEnumerator RotateDoor()
        {
            var startRotation = transform.localRotation;
            var endRotation = Quaternion.Euler(0, targetAngle, 0);
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = endRotation;
        }
    }
}