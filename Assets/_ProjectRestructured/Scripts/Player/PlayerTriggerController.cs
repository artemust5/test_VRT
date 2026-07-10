using System;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Player {
  public class PlayerTriggerController : MonoBehaviour {
    [SerializeField] private string boundaryTag = "Boundary";
    [SerializeField] private string hazardTag = "Hazard";
    [SerializeField] private string obstacleTag = "Obstacle";
    [SerializeField] private string completeTag = "Complete";

    public Action OnCrash;
    public Action OnExitBoundary;
    public Action OnObstacle;
    public Action OnComplete;

    private void OnTriggerEnter(Collider other) {
      CheckCollisionTags(other.tag);
    }

    private void OnCollisionEnter(Collision collision) {
      CheckCollisionTags(collision.gameObject.tag);
    }

    private void CheckCollisionTags(string objectTag) {
      if (objectTag == boundaryTag)
        OnExitBoundary?.Invoke();

      if (objectTag == hazardTag)
        OnCrash?.Invoke();

      if (objectTag == obstacleTag)
        OnObstacle?.Invoke();

      if (objectTag == completeTag)
        OnComplete?.Invoke();
    }
  }
}