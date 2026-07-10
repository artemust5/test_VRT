using UnityEngine;
using UnityEngine.Events;

public class PathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;
    public bool loopPath = false;

    [Header("Events")]
    public UnityEvent onPathComplete;

    private int currentIndex = 0;
    private bool pathCompleted = false;

    void Update()
    {
        if (pathCompleted || waypoints == null || waypoints.Length == 0)
            return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;
        }
        else
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                if (loopPath)
                {
                    currentIndex = 0;
                }
                else
                {
                    pathCompleted = true;
                    onPathComplete.Invoke();
                }
            }
        }
    }
}