using System;
using UnityEngine;

namespace _Project._Scripts
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private GameObject obstacle;
        [SerializeField] private BicycleVehicle bicycleVehicle;
        [SerializeField] private GameObject resultUI;

        public event Action OnSegmentEnd;
        private bool _isBicycleVehicleNotNull;

        private void Start()
        {
            _isBicycleVehicleNotNull = bicycleVehicle != null;
        }

        private void Update()
        {
            if (!_isBicycleVehicleNotNull || bicycleVehicle.IsMoving() || !obstacle.activeSelf) return;

            bicycleVehicle.EndSegment();
            OnSegmentEnd?.Invoke();
            resultUI.SetActive(true);
        }

        public void ActivateObstacle()
        {
            if (obstacle == null)
            {
                Debug.LogError("Obstacle is not assigned.");
                return;
            }

            obstacle.SetActive(true);
            StartSegment();
            Debug.Log("Start Segment");
        }

        private void StartSegment()
        {
            if (bicycleVehicle == null)
            {
                Debug.LogError("BicycleVehicle is not assigned.");
                return;
            }

            bicycleVehicle.StartSegment();
        }
    }
}