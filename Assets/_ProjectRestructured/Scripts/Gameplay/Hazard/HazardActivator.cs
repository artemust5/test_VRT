#if UNITY_EDITOR
using UnityEditor;
#endif
using _ProjectRestructured.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace _ProjectRestructured.Scripts.Gameplay.Hazard {
  public class HazardActivator : MonoBehaviour {
    [Header("Timing Settings")] public float obstacleTimeToCollision = 2f;
    public float additionalTimeBeforeCollision = 2f;
    private const float FewSecondsBeforeTimeToCollision = 3f;

    [Header("Events")] public UnityEvent onRequiredDistance;
    public UnityEvent onAdditionalPreTrigger;
    [HideInInspector] public UnityEvent onFewSecondsBeforeActivation;

    [Header("Debug Visualization")] public bool enableDebugGizmos;

    private BaseVehicleController playerController;
    private Transform player;
    private bool onRequiredInvoked;
    private bool onAdditionalInvoked;
    private bool onFewSecondsBeforeRequiredInvoked;

    private Vector3 fewSecondsBeforePos;
    private Vector3 requiredDistancePos;
    private Vector3 additionalTriggerPos;
    private float lastPlayerSpeedKph;
    private float lastRequiredDistance;
    private float lastAdditionalDistance;

    private void Start() {
      playerController = VehicleManager.Instance.Current.controller;
      player = playerController.transform;
    }

    private void Update() {
      if (!player) return;

      var rb = playerController.GetComponent<Rigidbody>();
      var playerSpeedMps = rb != null ? rb.velocity.magnitude : 0f;
      var playerSpeedKph = playerSpeedMps * 3.6f;

      var distanceToIntersection = Vector3.Distance(player.position, transform.position);

      var requiredDistance = playerSpeedMps * obstacleTimeToCollision;
      var fewSecondsBeforeDistance = playerSpeedMps * (obstacleTimeToCollision + FewSecondsBeforeTimeToCollision);
      var additionalTriggerDistance = playerSpeedMps * (obstacleTimeToCollision + additionalTimeBeforeCollision);

      if (distanceToIntersection <= requiredDistance && !onRequiredInvoked) {
        requiredDistancePos = player.position;
        lastRequiredDistance = requiredDistance;
        lastPlayerSpeedKph = playerSpeedKph;

        onRequiredDistance?.Invoke();
        onRequiredInvoked = true;
      }

      if (distanceToIntersection <= additionalTriggerDistance && !onAdditionalInvoked) {
        additionalTriggerPos = player.position;
        lastAdditionalDistance = additionalTriggerDistance;
        onAdditionalPreTrigger?.Invoke();
        onAdditionalInvoked = true;
      }

      if (!(distanceToIntersection <= fewSecondsBeforeDistance) || onFewSecondsBeforeRequiredInvoked) return;
      fewSecondsBeforePos = player.position;
      onFewSecondsBeforeActivation?.Invoke();
      onFewSecondsBeforeRequiredInvoked = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
      if (!enableDebugGizmos) return;

      var style = new GUIStyle(EditorStyles.boldLabel) {
        fontSize = 11,
        normal = { textColor = Color.white }
      };

      const float yOffset = 1.5f;

      if (fewSecondsBeforePos != Vector3.zero) {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(fewSecondsBeforePos, 0.5f);

        var label = $"[Early Trigger]\nSpeed: {lastPlayerSpeedKph:F1} km/h";
        Handles.Label(fewSecondsBeforePos + Vector3.up * yOffset, label, style);
      }

      if (additionalTriggerPos != Vector3.zero) {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(additionalTriggerPos, 0.5f);

        var label = $"[Additional Trigger]\nDistance: {lastAdditionalDistance:F1} m\nSpeed: {lastPlayerSpeedKph:F1} km/h\n--- Actions ---\n{GetEventDescription(onAdditionalPreTrigger)}";
        Handles.Label(additionalTriggerPos + Vector3.up * yOffset, label, style);
      }

      if (requiredDistancePos != Vector3.zero) {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(requiredDistancePos, 0.5f);

        var label = $"[Required Trigger]\nSpeed: {lastPlayerSpeedKph:F1} km/h\nDistance: {lastRequiredDistance:F1} m\n--- Actions ---\n{GetEventDescription(onRequiredDistance)}";
        Handles.Label(requiredDistancePos + Vector3.up * yOffset, label, style);
      }

      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(transform.position, 0.5f);
      Handles.Label(transform.position + Vector3.up * yOffset, "[Hazard Target]", style);
    }

    private string GetEventDescription(UnityEvent unityEvent) {
      if (unityEvent == null) return "(No event)";
      var count = unityEvent.GetPersistentEventCount();
      if (count == 0) return "(No actions assigned)";
      var result = "";
      for (var i = 0; i < count; i++) {
        var target = unityEvent.GetPersistentTarget(i);
        var method = unityEvent.GetPersistentMethodName(i);
        if (target == null || string.IsNullOrEmpty(method)) continue;
        var objectName = (target as Component)?.gameObject.name ?? target.ToString();
        var componentType = target.GetType().Name;
        result += $"- {objectName}.{componentType}.{method}()\n";
      }

      return result.TrimEnd();
    }
#endif
  }
}