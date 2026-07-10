using UnityEngine;

namespace _Project._Scripts
{
    public class RayCastManager : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private GameObject targetObject;

        [Header("Detection Settings")] 
        [SerializeField] private LayerMask trafficLayer;

        [SerializeField] private float detectionDistance = 7f;
        [SerializeField] private float detectionRadius = 1f;
        [SerializeField] private float raycastOffset = 2.6f;
        [SerializeField] private float detectionInterval = 0.2f;

        [Header("Visualization Settings")] [SerializeField]
        private bool visualize = true;

        [SerializeField] private bool displayAllSpheres = true;

        private RaycastHit _hitInfo;
        private bool _hasHit;
        private RayCastInfo _rayCastInfo;

        private void Start()
        {
            _rayCastInfo = new RayCastInfo();
            StartCoroutine(PerformSphereCast());
        }

        private System.Collections.IEnumerator PerformSphereCast()
        {
            while (true)
            {
                var forward = targetObject.transform.forward;
                var raycastOrigin = targetObject.transform.position + forward * raycastOffset;
                var raycastDirection = forward * detectionDistance;

                _hasHit = Physics.SphereCast(raycastOrigin, detectionRadius, raycastDirection, out _hitInfo,
                    detectionDistance, trafficLayer);

                if (_hasHit)
                {
                    var distance = _hitInfo.distance;
                    _rayCastInfo.SetInfo(distance, true);
                }
                else
                {
                    _rayCastInfo.SetInfo(0, false);
                }

                yield return new WaitForSeconds(detectionInterval);
            }
        }

        public bool IsObjectDetected()
        {
            return _rayCastInfo.HasCollision;
        }

        public RayCastInfo GetRayCastInfo()
        {
            return _rayCastInfo;
        }

        private void OnDrawGizmos()
        {
            if (!visualize || targetObject == null) return;

            var forward = targetObject.transform.forward;
            var raycastOrigin = targetObject.transform.position + forward * raycastOffset;
            var raycastDirection = forward * detectionDistance;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(raycastOrigin, detectionRadius);
            Gizmos.DrawWireSphere(raycastOrigin + raycastDirection, detectionRadius);

            if (displayAllSpheres)
            {
                DrawSpheresAlongLine(raycastOrigin, raycastOrigin + raycastDirection, detectionRadius);
            }
            else
            {
                Gizmos.DrawLine(raycastOrigin, raycastOrigin + raycastDirection);
            }

            if (!_hasHit) return;

            Gizmos.color = Color.green;
            var bounds = _hitInfo.collider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        private static void DrawSpheresAlongLine(Vector3 start, Vector3 end, float radius)
        {
            var distance = Vector3.Distance(start, end);
            var numSpheres = Mathf.FloorToInt(distance / (radius * 2));
            var step = (end - start) / numSpheres;

            for (var i = 0; i <= numSpheres; i++)
            {
                var spherePosition = start + step * i;
                Gizmos.DrawWireSphere(spherePosition, radius);
            }
        }
    }
}