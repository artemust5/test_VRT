using _Project._Scripts;
using TMPro;
using UnityEngine;

public class ResultTextUI : MonoBehaviour
{
    public TMP_Text playerSpeed;
    public TMP_Text reactionTimeText;
    public TMP_Text reactionDistanceText;
    public TMP_Text brakingDistanceText;
    public TMP_Text stoppingDistanceText;

    public TMP_Text reactionTimeDelayedText;
    public TMP_Text reactionDistanceDelayedText;
    public TMP_Text brakingDistanceDelayedText;
    public TMP_Text stoppingDistanceDelayedText;
    public TMP_Text impactSpeed;

    private void Update()
    {
        playerSpeed.text = $"{DrivingAnalysis.Instance.DrivingSpeed:F1} km/h";
        reactionTimeText.text = $"{DrivingAnalysis.Instance.LastReactionTime:F1} s";
        reactionDistanceText.text = $"{DrivingAnalysis.Instance.LastReactionDistance:F1} m";
        brakingDistanceText.text = $"{DrivingAnalysis.Instance.LastBrakingDistance:F1} m";
        stoppingDistanceText.text = $"{DrivingAnalysis.Instance.LastStoppingDistance:F1} m";

        reactionTimeDelayedText.text = $"{DrivingAnalysis.Instance.LastImpairedReactionTime:F1} s";
        reactionDistanceDelayedText.text = $"{DrivingAnalysis.Instance.LastImpairedReactionDistance:F1} m";
        brakingDistanceDelayedText.text = $"{DrivingAnalysis.Instance.LastBrakingDistance:F1} m";
        stoppingDistanceDelayedText.text =
            $"{DrivingAnalysis.Instance.LastImpairedReactionDistance + DrivingAnalysis.Instance.LastBrakingDistance:F1} m";
        impactSpeed.text = $"{DrivingAnalysis.Instance.LastImpactSpeed:F1} km/h";
    }
}