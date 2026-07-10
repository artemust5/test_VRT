using TMPro;
using UnityEngine;

namespace _Project._Scripts
{
    public class TestNewAnalysis : MonoBehaviour
    {
        [SerializeField] private BicycleVehicle bicycleVehicle;

        [SerializeField] private TMP_Text reactionTime;
        [SerializeField] private TMP_Text reactionDistance;
        [SerializeField] private TMP_Text brakingDistance;
        [SerializeField] private TMP_Text stoppingDistance;
        [SerializeField] private TMP_Text obstacleDistance;

        private void Start()
        {
            DrivingAnalysis.Initialize();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            reactionTime.text = $"Reaction Time: {DrivingAnalysis.Instance.LastReactionTime:F1}s";
            reactionDistance.text = $"Reaction Distance: {DrivingAnalysis.Instance.LastReactionDistance:F1}m";
            brakingDistance.text = $"Braking Distance: {DrivingAnalysis.Instance.LastBrakingDistance:F1}m";
            stoppingDistance.text = $"Stopping Distance: {DrivingAnalysis.Instance.LastStoppingDistance:F1}m";
            obstacleDistance.text = $"Obstacle Distance: {DrivingAnalysis.Instance.LastObstacleDistance:F1}m";
        }
    }
}