using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Simple_Bicycle_Physics.Scripts;

namespace _ProjectRestructured.Scripts.Player
{
    public class NoSteeringController : MonoBehaviour
    {
        [Header("Waypoint Source")]
        [Tooltip("Parent object containing all waypoint transforms in order")]
        public Transform pointsRoot;

        [Header("Waypoint Settings")]
        [Tooltip("Distance threshold to switch to next waypoint")]
        public float waypointThreshold = 1f;

        [Header("Rotation Settings")]
        [Tooltip("Rotation speed in degrees/sec")]
        public float rotationSpeed = 90f;
        [Tooltip("Final alignment threshold (degrees)")]
        [Range(0.1f, 5f)] public float finalizeThreshold = 1f;
        [Tooltip("Rotation smoothing factor (0-1)")]
        [Range(0.01f, 0.5f)] public float rotationSmoothing = 0.1f;

        private List<Transform> waypoints = new();
        private Quaternion targetRotation;
        private bool needsRotation;
        private int currentWaypointIndex;
        private Transform currentTarget;
        private BaseVehicleController bicycleVehicle;

        private void Awake()
        {
            if (pointsRoot == null)
            {
                enabled = false;
                return;
            }

            waypoints = new List<Transform>();
            foreach (Transform child in pointsRoot)
            {
                waypoints.Add(child);
            }

            if (waypoints.Count > 0)
            {
                currentTarget = waypoints[0];
                currentWaypointIndex = 0;
            }
            else
            {
                Debug.LogWarning("No waypoints found under Points object!");
                enabled = false;
            }
        }

        private void OnEnable()
        {
            bicycleVehicle = VehicleManager.Instance.Current.controller;
            bicycleVehicle.SetSteeringOnOff(false);
        }

        private void OnDisable()
        {
            if (bicycleVehicle != null)
                bicycleVehicle.SetSteeringOnOff(true);
        }

        private void Update()
        {
            if (currentTarget == null || bicycleVehicle == null) return;

            var vehicleTransform = bicycleVehicle.transform;

            var distanceToTarget = Vector3.Distance(vehicleTransform.position, currentTarget.position);
            if (distanceToTarget < waypointThreshold)
            {
                SwitchToNextWaypoint();
                if (currentTarget == null) return;
            }

            var direction = currentTarget.position - vehicleTransform.position;
            direction.y = 0;

            if (direction == Vector3.zero) return;

            targetRotation = Quaternion.LookRotation(direction);
            var angleToTarget = Quaternion.Angle(vehicleTransform.rotation, targetRotation);

            needsRotation = angleToTarget > finalizeThreshold;

            if (!needsRotation)
            {
                vehicleTransform.rotation = targetRotation;
                return;
            }

            ApplyRotation(vehicleTransform, angleToTarget);
        }

        private void SwitchToNextWaypoint()
        {
            currentWaypointIndex++;

            if (currentWaypointIndex < waypoints.Count)
            {
                currentTarget = waypoints[currentWaypointIndex];
            }
            else
            {
                currentTarget = null;
                Debug.Log("Route completed!");
            }
        }

        private void ApplyRotation(Transform vehicleTransform, float angleToTarget)
        {
            if (angleToTarget < 10f)
            {
                vehicleTransform.rotation = Quaternion.Slerp(
                    vehicleTransform.rotation,
                    targetRotation,
                    rotationSmoothing * 20 * Time.deltaTime);
            }
            else
            {
                vehicleTransform.rotation = Quaternion.RotateTowards(
                    vehicleTransform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        private void OnDrawGizmos()
        {
            if (pointsRoot == null) return;

            var children = pointsRoot.Cast<Transform>().ToList();

            Gizmos.color = Color.cyan;
            for (var i = 0; i < children.Count - 1; i++)
            {
                if (children[i] != null && children[i + 1] != null)
                    Gizmos.DrawLine(children[i].position, children[i + 1].position);
            }

            if (currentTarget == null || bicycleVehicle == null) return;
            Gizmos.color = needsRotation ? Color.yellow : Color.green;
            Gizmos.DrawLine(bicycleVehicle.transform.position, currentTarget.position);
        }
    }
}
