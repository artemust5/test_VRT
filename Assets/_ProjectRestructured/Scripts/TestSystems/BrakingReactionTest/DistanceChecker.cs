using _ProjectRestructured.Scripts.Player;
using UnityEngine;

namespace _ProjectRestructured.Scripts.TestSystems.BrakingReactionTest
{
    public class DistanceChecker : MonoBehaviour
    {
        public Collider targetCollider;
        private Collider playerCollider;

        private void Start()
        {
            playerCollider = VehicleManager.Instance.Current.bodyCollider;
            
            if (!playerCollider || !targetCollider)
                Debug.LogError($"Collider isn't attached to '{name}'!");
        }

        public float GetDistance()
        {
            if (!playerCollider || !targetCollider) return 0f;
            var closestPointOnMyCollider = playerCollider.ClosestPoint(targetCollider.transform.position);
            var closestPointOnTarget = targetCollider.ClosestPoint(closestPointOnMyCollider);

            var minDistance = Vector3.Distance(closestPointOnMyCollider, closestPointOnTarget);
            return minDistance;
        }
    }
}