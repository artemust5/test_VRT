using System.Collections;
using _ProjectRestructured.Scripts.Gameplay.Hazard;
using _ProjectRestructured.Scripts.Player;
using _ProjectRestructured.Scripts.TestSystems.BrakingReactionTest;
using _ProjectRestructured.Scripts.UI;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Gameplay {
  public class GameplayManager : MonoBehaviour {
    [SerializeField] private HazardActivator hazardActivator;
    [SerializeField] private ReplayManager.ReplayManager replayManager;
    [SerializeField] private BrakingReactionTestCalculator brakingReactionTestCalculator;
    [SerializeField] private MenuScreensController menuScreensController;
    [SerializeField] private PauseManager pauseManager;

    private PlayerTriggerController playerTriggerController;

    private void OnEnable() {
      hazardActivator.onRequiredDistance.AddListener(OnHazardActivation);
      hazardActivator.onFewSecondsBeforeActivation.AddListener(StartRecordingReplay);
      brakingReactionTestCalculator.OnBrakingResultsCalculated += OnSuccessfulFinish;
      brakingReactionTestCalculator.OnBrakingResultsCalculatedAfterCrash += OnFailedFinish;
    }

    private void Start() {
      SubscribePlayerTriggerEvents();
      menuScreensController.ActivateStartScreen();
      pauseManager.Initialize();
      FreezePlayer();
    }

    private void SubscribePlayerTriggerEvents() {
      playerTriggerController = VehicleManager.Instance.Current.rootObject.GetComponent<PlayerTriggerController>();
      if (playerTriggerController != null) {
        playerTriggerController.OnCrash += HandleCrashing;
        playerTriggerController.OnExitBoundary += HandleExitBoundary;
        playerTriggerController.OnObstacle += HandleCrashInObstacle;
        playerTriggerController.OnComplete += HandleCompleteSituation;
      }
      else Debug.LogError("GameplayManager didn't find the PlayerTriggerController component on the current vehicle in VehicleManager");
    }

    public void StartGame() {
      menuScreensController.CloseStartScreen();
      UnFreezePlayer();
      pauseManager.ResumeGame();
    }

    private void HandleExitBoundary() {
      FreezePlayer();
      pauseManager.PauseGame();
      menuScreensController.ActivateBoundaryScreen();
    }

    private void HandleCrashing() {
      if (brakingReactionTestCalculator.testStarted) {
        playerTriggerController.OnCrash -= HandleCrashing;
        brakingReactionTestCalculator.MakeCalculationsAfterCrash();
        return;
      }

      FreezePlayer();
      pauseManager.PauseGame();
      menuScreensController.ActivateCrashScreen();
    }

    private void HandleCrashInObstacle() {
      FreezePlayer();
      pauseManager.PauseGame();
      menuScreensController.ActivateCrashScreen();
    }

    private void HandleCompleteSituation() {
      FreezePlayer();
      pauseManager.PauseGame();
      menuScreensController.ActivateCompleteScreen();
    }

    private void OnHazardActivation() {
      brakingReactionTestCalculator.StartTest();
    }

    public void OnSuccessfulFinish() {
      playerTriggerController.OnExitBoundary -= HandleExitBoundary;
      StartCoroutine(SuccessfulFinish());
    }

    public void OnFailedFinish() {
      playerTriggerController.OnExitBoundary -= HandleExitBoundary;
      StartCoroutine(FailedFinish());
    }

    private IEnumerator SuccessfulFinish() {
      FreezePlayer();
      yield return new WaitForSeconds(2f);

      replayManager.StopRecording();
      replayManager.SaveReplay();

      pauseManager.PauseGame();
      menuScreensController.ActivateFinishScreenSuccessful();
    }

    private IEnumerator FailedFinish() {
      yield return new WaitForSeconds(2f);
      FreezePlayer();

      replayManager.StopRecording();
      replayManager.SaveReplay();

      pauseManager.PauseGame();
      menuScreensController.ActivateFinishScreenFailed();
    }

    private void FreezePlayer() {
      VehicleManager.Instance.Current.controller.enabled = false;
      VehicleManager.Instance.Current.controller.rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void UnFreezePlayer() {
      VehicleManager.Instance.Current.controller.enabled = true;
      VehicleManager.Instance.Current.controller.rb.constraints = RigidbodyConstraints.None;
    }

    private void StartRecordingReplay() {
      replayManager.StartRecording();
      Debug.LogError($"Start Replay!");
    }
  }
}