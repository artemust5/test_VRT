using UnityEngine;

namespace _ProjectRestructured.Scripts.UI
{
    public class VrUIFollowHead : MonoBehaviour
    {
        public float distanceFromHead = 0.5f;
        public float smoothSpeed = 5f;
        public bool followY;
        [SerializeField] private Transform targetCamera;

        private float offsetY;

        private void Start()
        {
            if (!targetCamera)
                Debug.LogError($"No camera attached to {name}");

            if (followY)
            {
                offsetY = targetCamera.position.y - transform.position.y;
            }
        }

        private void LateUpdate()
        {
            if (!targetCamera) return;
            var targetPosition = targetCamera.position + targetCamera.forward * distanceFromHead;
            var newPosition = new Vector3(targetPosition.x, followY? targetPosition.y - offsetY  :transform.position.y, targetPosition.z);
            var targetRotation = Quaternion.LookRotation(newPosition - targetCamera.position);

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * smoothSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }
}