// using System.Collections;
// using UnityEngine;
//
// namespace _Project._Scripts
// {
//     public class ObstacleSpawner : MonoBehaviour
//     {
//         [SerializeField] private GameObject obstacle;
//         [SerializeField] private Transform obstacleTransform;
//         [SerializeField] private BicycleVehicle bicycleVehicle;
//         [SerializeField] private Transform playerTransform;
//         [SerializeField] private float spawnDistance = 10f;
//         [SerializeField] private float minWaitTime = 5f;
//         [SerializeField] private float maxWaitTime = 10f;
//
//         private bool _obstacleSpawned;
//         private bool _isPlayerTransformNull;
//         private bool _isObstacleTransformNull;
//         private bool _isBicycleVehicleNull;
//
//         private void Start()
//         {
//             if (CheckNull()) return;
//
//             obstacle.SetActive(false);
//
//             StartCoroutine(RandomSpawnObstacle());
//         }
//
//         private bool CheckNull()
//         {
//             _isBicycleVehicleNull = bicycleVehicle == null;
//             _isObstacleTransformNull = obstacleTransform == null;
//             _isPlayerTransformNull = playerTransform == null;
//
//             if (bicycleVehicle == null)
//             {
//                 Debug.LogError("BicycleVehicle is not assigned.");
//                 return true;
//             }
//
//             if (obstacle == null || obstacleTransform == null)
//             {
//                 Debug.LogError("Obstacle or Obstacle Transform is not assigned.");
//                 return true;
//             }
//
//             return false;
//         }
//
//         private void Update()
//         {
//             if (_isBicycleVehicleNull || bicycleVehicle.IsMoving() || !obstacle.activeSelf) return;
//
//             CalculateAndSetObstacleDistance();
//             bicycleVehicle.EndSegment();
//
//             // if (playerTransform != null && obstacleTransform != null && obstacle.activeSelf)
//             // {
//             //     float distance = CalculateDistance(playerTransform.position, obstacleTransform.position);
//             //     Debug.Log("Distance to Obstacle: " + distance);  // Log distance for debugging
//             // }
//         }
//
//         private IEnumerator RandomSpawnObstacle()
//         {
//             var waitTime = Random.Range(minWaitTime, maxWaitTime);
//             yield return new WaitForSeconds(waitTime);
//
//             if (_obstacleSpawned) yield break;
//
//             obstacle.transform.position = playerTransform.position + playerTransform.forward * spawnDistance;
//             obstacle.SetActive(true);
//             bicycleVehicle.StartSegment();
//             _obstacleSpawned = true;
//         }
//
//         private void OnTriggerEnter(Collider other)
//         {
//             if (obstacleTransform == null || other.transform != obstacleTransform ||
//                 !other.CompareTag("Obstacle")) return;
//
//             CalculateAndSetObstacleDistance();
//             bicycleVehicle.EndSegment();
//         }
//
//         private void CalculateAndSetObstacleDistance()
//         {
//             if (_isPlayerTransformNull || _isObstacleTransformNull) return;
//
//             var distance = CalculateDistance(playerTransform.position, obstacleTransform.position);
//             DrivingAnalysis.Instance.SetObstacleDistance(distance);
//         }
//
//         private static float CalculateDistance(Vector3 point1, Vector3 point2)
//         {
//             var point1Flat = new Vector3(point1.x, 0, point1.z);
//             var point2Flat = new Vector3(point2.x, 0, point2.z);
//             return Vector3.Distance(point1Flat, point2Flat);
//         }
//     }
// }