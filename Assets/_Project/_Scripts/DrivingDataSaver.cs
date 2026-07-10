using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace _Project._Scripts
{
    public static class DrivingDataSaver
    {
        private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "driving_data.json");

        public static void SaveSession()
        {
            if (DrivingAnalysis.Instance == null)
            {
                Debug.LogError("[DrivingDataSaver] DrivingAnalysis.Instance is null!");
                return;
            }

            try
            {
                string json = File.Exists(FilePath) ? File.ReadAllText(FilePath) : "{\"sessions\":[]}";
                SimpleDataContainer data = JsonUtility.FromJson<SimpleDataContainer>(json);

                SessionData newSession = new SessionData
                {
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    drivingSpeed = DrivingAnalysis.Instance.DrivingSpeed,
                    reactionTime = DrivingAnalysis.Instance.LastReactionTime,
                    reactionDistance = DrivingAnalysis.Instance.LastReactionDistance,
                    brakingDistance = DrivingAnalysis.Instance.LastBrakingDistance,
                    stoppingDistance = DrivingAnalysis.Instance.LastStoppingDistance,
                    obstacleDistance = DrivingAnalysis.Instance.LastObstacleDistance,
                    impairedReactionTime = DrivingAnalysis.Instance.LastImpairedReactionTime,
                    impairedReactionDistance = DrivingAnalysis.Instance.LastImpairedReactionDistance,
                    impactSpeed = DrivingAnalysis.Instance.LastImpactSpeed,
                    distanceToObstacle = DrivingAnalysis.Instance.DistanceToObstacle,
                    crashed = DrivingAnalysis.Instance.Crashed
                };

                data.sessions.Add(newSession);

                string updatedJson = SerializeToJson(data);
                File.WriteAllText(FilePath, updatedJson);

                Debug.Log($"[DrivingDataSaver] Session saved! Total: {data.sessions.Count} | Path: {FilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DrivingDataSaver] Error: {e.Message}");
            }
        }

        public static string GetFilePath()
        {
            return FilePath;
        }

        private static string SerializeToJson(SimpleDataContainer data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"sessions\": [");

            for (int i = 0; i < data.sessions.Count; i++)
            {
                SessionData s = data.sessions[i];
                sb.AppendLine("    {");
                sb.AppendLine($"      \"timestamp\": \"{s.timestamp}\",");
                sb.AppendLine($"      \"drivingSpeed\": {s.drivingSpeed:F2},");
                sb.AppendLine($"      \"reactionTime\": {s.reactionTime:F2},");
                sb.AppendLine($"      \"reactionDistance\": {s.reactionDistance:F2},");
                sb.AppendLine($"      \"brakingDistance\": {s.brakingDistance:F2},");
                sb.AppendLine($"      \"stoppingDistance\": {s.stoppingDistance:F2},");
                sb.AppendLine($"      \"obstacleDistance\": {s.obstacleDistance:F2},");
                sb.AppendLine($"      \"impairedReactionTime\": {s.impairedReactionTime:F2},");
                sb.AppendLine($"      \"impairedReactionDistance\": {s.impairedReactionDistance:F2},");
                sb.AppendLine($"      \"impactSpeed\": {s.impactSpeed:F2},");
                sb.AppendLine($"      \"distanceToObstacle\": {s.distanceToObstacle:F2},");
                sb.AppendLine($"      \"crashed\": {s.crashed.ToString().ToLower()}");
                
                if (i < data.sessions.Count - 1)
                    sb.AppendLine("    },");
                else
                    sb.AppendLine("    }");
            }

            sb.AppendLine("  ]");
            sb.Append("}");

            return sb.ToString();
        }
    }

    [Serializable]
    public class SimpleDataContainer
    {
        public System.Collections.Generic.List<SessionData> sessions = new System.Collections.Generic.List<SessionData>();
    }

    [Serializable]
    public class SessionData
    {
        public string timestamp;
        public float drivingSpeed;
        public float reactionTime;
        public float reactionDistance;
        public float brakingDistance;
        public float stoppingDistance;
        public float obstacleDistance;
        public float impairedReactionTime;
        public float impairedReactionDistance;
        public float impactSpeed;
        public float distanceToObstacle;
        public bool crashed;
    }
}

