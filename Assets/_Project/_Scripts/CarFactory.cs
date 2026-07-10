using System.Collections.Generic;
using UnityEngine;

public class CarFactory : MonoBehaviour
{
    public GameObject[] carPrefabs; // Array to hold car prefabs.
    public Transform[] waypoints; // Array to hold waypoint transforms directly.
    public int quantity = 5; // Number of cars to instantiate.

    void Start()
    {
        SpawnCars();
    }

    void SpawnCars()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints set in CarFactory.");
            return; // Early exit if no waypoints are defined.
        }

        // Create a list to track available waypoints if you plan to remove them upon use
        List<Transform> availableWaypoints = new List<Transform>(waypoints);

        for (int i = 0; i < quantity; i++)
        {
            if (availableWaypoints.Count == 0) return; // Safety check

            // Select a random waypoint from the available list
            int waypointIndex = Random.Range(0, availableWaypoints.Count);
            Transform waypoint = availableWaypoints[waypointIndex];

            // Select a random car prefab
            int carIndex = Random.Range(0, carPrefabs.Length);
            GameObject carPrefab = carPrefabs[carIndex];

            // Instantiate the car at the waypoint
            Instantiate(carPrefab, waypoint.position, Quaternion.Euler(0, 90, 0));

            // Optional: Remove the waypoint from the list if you don't want multiple cars at the same waypoint
            availableWaypoints.RemoveAt(waypointIndex);
        }
    }
}