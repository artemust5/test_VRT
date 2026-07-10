using UnityEngine;

namespace _Project._Scripts
{
    public class DrivingAnalysis
    {
        public static DrivingAnalysis Instance { get; private set; }

        public float DrivingSpeed { get; private set; }
        public float LastReactionTime { get; private set; }
        public float LastReactionDistance { get; private set; }
        public float LastBrakingDistance { get; private set; }
        public float LastStoppingDistance { get; private set; }
        public float LastObstacleDistance { get; private set; }
        public float LastImpairedReactionTime { get; private set; }
        public float LastImpairedReactionDistance { get; private set; }
        public float LastImpactSpeed { get; private set; }

        public float DistanceToObstacle { get; private set; }

        public bool Crashed { get; private set; }

        private DrivingAnalysis()
        {
        }

        public static void Initialize()
        {
            Instance ??= new DrivingAnalysis();
            Debug.Log("Driving Analysis Initialization: Success");
        }

        public void RegisterSegmentResults(float drivingSpeed, float reactionTime, float reactionDistance, float brakingDistance,
            float stoppingDistance, float obstacleDistance, float impairedReactionTime, float impairedReactionDistance,
            float impactSpeed, float distanceToObstacle, bool crashed)
        {
            DrivingSpeed = drivingSpeed;
            LastReactionTime = reactionTime;
            LastReactionDistance = reactionDistance;
            LastBrakingDistance = brakingDistance;
            LastStoppingDistance = stoppingDistance;
            LastObstacleDistance = obstacleDistance;
            LastImpairedReactionTime = impairedReactionTime;
            LastImpairedReactionDistance = impairedReactionDistance;
            LastImpactSpeed = impactSpeed;
            DistanceToObstacle = distanceToObstacle;
            Crashed = crashed;
            DrivingDataSaver.SaveSession();
        }

        public void UpdateImpairedResults(float impairedReactionTime, float impairedReactionDistance)
        {
            LastImpairedReactionTime = impairedReactionTime;
            LastImpairedReactionDistance = impairedReactionDistance;
        }

        public void UpdateImpactSpeedResult(float impactSpeed)
        {
            LastImpactSpeed = impactSpeed;
        }
    }
}