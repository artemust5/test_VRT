using System;
using System.Collections;
using _Project._Scripts;
using _ProjectRestructured.Scripts.Player;
using UnityEngine;

namespace _ProjectRestructured.Scripts.TestSystems.BrakingReactionTest {
  public class BrakingReactionTestCalculator : MonoBehaviour {
    public float startWithDelay;

    private float startTestTime, startBrakingTime;
    private Vector3 startTestPosition;
    private Vector3? startBrakingPosition;
    private float speedBeforeBraking;
    private bool testFinished, startBraking;

    [SerializeField] private DistanceChecker distanceChecker;
    private BaseVehicleController player;

    [HideInInspector] public bool testStarted;
    public Action OnBrakingResultsCalculated;
    public Action OnBrakingResultsCalculatedAfterCrash;

    private void Start() {
      player = VehicleManager.Instance.Current.controller;
      DrivingAnalysis.Initialize();
    }

    private void Update() {
      if (!testStarted || testFinished) return;
      if (player.IsBraking && !startBraking) {
        startBraking = true;
        StartBrakingCalculations();
      }
      if (!startBraking || player.CurrentSpeed >= 0.04f) return;
      MakeCalculations();
      testFinished = true;
      OnBrakingResultsCalculated?.Invoke();
      enabled = false;
    }

    public void StartTest() {
      if (startWithDelay > 0) {
        StartCoroutine(StartWithDelay());
        return;
      }

      startTestTime = Time.time;
      startTestPosition = player.rb.position;
      speedBeforeBraking = player.CurrentSpeed;
      testStarted = true;
    }

    private IEnumerator StartWithDelay() {
      yield return new WaitForSeconds(startWithDelay);
      startWithDelay = 0;
      StartTest();
    }

    private void StartBrakingCalculations() {
      startBrakingTime = Time.time;
      startBrakingPosition = player.rb.position;
    }

    private void MakeCalculations() {
      const int drunkDelay = 1;
      var obstacleDistance = distanceChecker.GetDistance();
      var reactionDistance = Vector3.Distance(startTestPosition, player.rb.position);

      float reactionTime = 0, impairedReactionTime = 0, impairedReactionDistance = 0, brakingDistance = 0, stoppingDistance = 0;
      if (startBrakingPosition.HasValue) {
        reactionDistance = Vector3.Distance(startTestPosition, startBrakingPosition.Value);
        brakingDistance = Vector3.Distance(startBrakingPosition.Value, player.rb.position);
        stoppingDistance = reactionDistance + brakingDistance;
        reactionTime = startBrakingTime - startTestTime;
        impairedReactionTime = reactionTime + drunkDelay;
        impairedReactionDistance = impairedReactionTime * (speedBeforeBraking / 3.6f);
      }

      DrivingAnalysis.Instance.RegisterSegmentResults(speedBeforeBraking, reactionTime, reactionDistance, brakingDistance,
          stoppingDistance, obstacleDistance, impairedReactionTime, impairedReactionDistance, 0f, obstacleDistance, false);
    }

    public void MakeCalculationsAfterCrash() {
      testFinished = true;
      const int drunkDelay = 1;
      var impactSpeed = player.CurrentSpeed;
      var reactionTime = startBrakingPosition.HasValue ? Time.time - startTestTime : 0;
      var reactionDistance = Vector3.Distance(startTestPosition, player.rb.position);
      var impairedReactionTime = reactionTime + drunkDelay;
      var impairedReactionDistance = startBrakingPosition.HasValue ? reactionDistance * (impairedReactionTime / reactionTime) : reactionDistance;
      var brakingDistance = startBrakingPosition.HasValue ? Vector3.Distance(startBrakingPosition.Value, player.rb.position) : 0;

      DrivingAnalysis.Instance.RegisterSegmentResults(speedBeforeBraking, reactionTime, reactionDistance, brakingDistance,
          0, 0, impairedReactionTime, impairedReactionDistance, impactSpeed, 0f, true);

      OnBrakingResultsCalculatedAfterCrash?.Invoke();
    }
  }
}