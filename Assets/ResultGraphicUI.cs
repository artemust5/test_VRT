using _Project._Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ResultGraphicUI : MonoBehaviour
{
    public RectTransform mainPanel;
    public RectTransform reactionPanel;
    public RectTransform brakingPanel;
    public RectTransform obstacleMarker;
    public RectTransform obstacleExplosionImage;

    public Text reactionTimeText;
    public Text reactionDistanceText;
    public Text brakingDistanceText;
    public Text obstacleDistanceText;
    public Text impactSpeedText;

    [SerializeField] private Obstacle obstacle;
    [SerializeField] private bool drunkMode;

    private float localScaleFactor = 1f;
    private static float _scaleFactor = 1f;
    private const float MinScaleFactor = 150f;
    private const float MinWidth = 150f;
    private float normalModeObstacleMarkerX;
    private float drunkDelay = 1f;
    private readonly float[] drunkDelayArray = { 0.2f, 0.5f, 0.8f, 1.0f, 1.5f };
    private int drunkDelayIndex = -1;

    private void Awake()
    {
        UpdateResultsBasedOnState();
    }

    private void Update()
    {
        if (Mathf.Approximately(localScaleFactor, _scaleFactor)) return;
        localScaleFactor = _scaleFactor;
        UpdateResultsBasedOnState();
    }

    private void UpdateResultsBasedOnState()
    {
        if (drunkMode)
        {
            DisplayImpairedResults();
        }
        else
        {
            DisplayNormalResults();
        }
    }

    public void DisplayNormalResults()
    {
        drunkMode = false;
        UpdateUI(DrivingAnalysis.Instance.LastReactionTime, DrivingAnalysis.Instance.LastReactionDistance,
            DrivingAnalysis.Instance.LastBrakingDistance, DrivingAnalysis.Instance.LastObstacleDistance);
        SetMarkerPosition();
        obstacleMarker.anchoredPosition = new Vector2(normalModeObstacleMarkerX, obstacleMarker.anchoredPosition.y);
    }

    public void DisplayImpairedResults()
    {
        drunkMode = true;
        UpdateUI(DrivingAnalysis.Instance.LastImpairedReactionTime,
            DrivingAnalysis.Instance.LastImpairedReactionDistance,
            DrivingAnalysis.Instance.LastBrakingDistance, 0);
        SetMarkerPosition();
        obstacleMarker.anchoredPosition = new Vector2(normalModeObstacleMarkerX, obstacleMarker.anchoredPosition.y);
        
        if (!DrivingAnalysis.Instance.Crashed)
            RecalculateImpactSpeed();
    }

    private void RecalculateImpactSpeed()
    {
        var initialSpeed = DrivingAnalysis.Instance.DrivingSpeed;
        var distanceToObstacle = DrivingAnalysis.Instance.DistanceToObstacle;
        var brakingDistance = DrivingAnalysis.Instance.LastBrakingDistance;
        var normalReactionDistance = DrivingAnalysis.Instance.LastReactionDistance;
        var impairedReactionTime = DrivingAnalysis.Instance.LastReactionTime + drunkDelay;

        var totalDistance = normalReactionDistance + brakingDistance + distanceToObstacle;
        var impairedReactionDistance = impairedReactionTime * (initialSpeed / 3.6f);
        
        if (impairedReactionDistance + brakingDistance < totalDistance) // won't crash at all
        {
            impactSpeedText.text =  $"{0f:F1}km/h";
            DrivingAnalysis.Instance.UpdateImpactSpeedResult(0f);
            obstacleExplosionImage.gameObject.SetActive(false);
            return;
        }
        var remainingDistanceToObstacle = totalDistance - impairedReactionDistance;
        if (remainingDistanceToObstacle <= 0) // will crash before starting to brake
        {
            impactSpeedText.text =  $"{initialSpeed:F1}km/h";
            DrivingAnalysis.Instance.UpdateImpactSpeedResult(initialSpeed);
            obstacleExplosionImage.gameObject.SetActive(true);
            return;
        }
        var brakingAcceleration = Mathf.Pow(initialSpeed / 3.6f, 2) / (2 * brakingDistance);
        var impactSpeedSquared = Mathf.Pow(initialSpeed / 3.6f, 2) - 2 * brakingAcceleration * remainingDistanceToObstacle;
        var impactSpeed = Mathf.Sqrt(impactSpeedSquared) * 3.6f;
        impactSpeed *= 0.8f;

        impactSpeedText.text =  $"{impactSpeed:F1}km/h";
        DrivingAnalysis.Instance.UpdateImpactSpeedResult(impactSpeed);
        obstacleExplosionImage.gameObject.SetActive(true);
    }

    public void SwitchDrunkDelay()
    {
        drunkDelay = drunkDelayArray[++drunkDelayIndex % drunkDelayArray.Length];
        RecalculateImpairedResults();
    }

    private void RecalculateImpairedResults()
    {
        var impairedReactionTime = DrivingAnalysis.Instance.LastReactionTime + drunkDelay;
        var impairedReactionDistance = impairedReactionTime * (DrivingAnalysis.Instance.DrivingSpeed / 3.6f);
        
        DrivingAnalysis.Instance.UpdateImpairedResults(impairedReactionTime, impairedReactionDistance);
        UpdateResultsBasedOnState();
    }

    private void UpdateUI(float reactionTime, float reactionDistance, float brakingDistance, float obstacleDistance)
    {
        var reactionPanelWidth = Mathf.Max(reactionDistance * MinScaleFactor, MinWidth);
        var brakingPanelWidth = Mathf.Approximately(brakingDistance, 0f) ? 0f : 
            Mathf.Max((DrivingAnalysis.Instance.Crashed? CalculateFullBrakingDistance():brakingDistance) * MinScaleFactor, MinWidth);

        if (drunkMode)
        {
            var totalWidth = reactionPanelWidth + brakingPanelWidth + MinWidth;
            var availableWidth = mainPanel.rect.width;
        
            if (totalWidth > availableWidth)
                _scaleFactor = availableWidth / totalWidth;
            else
                _scaleFactor = 1f;
        }
    
        reactionPanelWidth *= _scaleFactor;
        brakingPanelWidth *= _scaleFactor;
    
        reactionPanel.sizeDelta = new Vector2(reactionPanelWidth, reactionPanel.sizeDelta.y);
        brakingPanel.sizeDelta = new Vector2(brakingPanelWidth, brakingPanel.sizeDelta.y);
        brakingPanel.anchoredPosition = new Vector2(reactionPanelWidth, brakingPanel.anchoredPosition.y);

        if ( Mathf.Approximately(brakingDistance, 0f))
        {
            reactionTimeText.text = "";
            reactionDistanceText.text = $"{reactionDistance:F1}m";
            brakingDistanceText.text = "";
            obstacleDistanceText.text = "";
            return;
        }
        reactionTimeText.text = $"{reactionTime:F1}s";
        reactionDistanceText.text = $"{reactionDistance:F1}m";
        brakingDistanceText.text = $"{brakingDistance:F1}m";
        obstacleDistanceText.text = $"{obstacleDistance:F1}m";
    }

    private void SetMarkerPosition()
    {
        normalModeObstacleMarkerX = CalculateObstacleMarkerPosition(DrivingAnalysis.Instance.LastReactionDistance,
            DrivingAnalysis.Instance.LastBrakingDistance, DrivingAnalysis.Instance.LastObstacleDistance);
    }

    private float CalculateObstacleMarkerPosition(float reactionDistance, float brakingDistance, float obstacleDistance)
    {
        var reactionPanelWidth = Mathf.Max(reactionDistance * MinScaleFactor, MinWidth);
    
        // Используем ту же логику, что и в UpdateUI
        var brakingPanelWidth = Mathf.Approximately(brakingDistance, 0f) ? 0f : 
            Mathf.Max((DrivingAnalysis.Instance.Crashed? CalculateFullBrakingDistance():brakingDistance) * MinScaleFactor, MinWidth);
    
        var obstaclePanelWidth = obstacleDistance > 0 ? Mathf.Max(obstacleDistance * MinScaleFactor, MinWidth) : 0f;

        var totalX = (reactionPanelWidth + brakingPanelWidth + obstaclePanelWidth) * _scaleFactor;
        var finalPos = Mathf.Min(totalX, mainPanel.rect.width - obstacleMarker.rect.width);

        return finalPos;
    }

    
    private float CalculateFullBrakingDistance()
    {
        var initialSpeedKmh = DrivingAnalysis.Instance.DrivingSpeed;
        var impactSpeedKmh = DrivingAnalysis.Instance.LastImpactSpeed;
        var brakingDistanceBeforeImpact = DrivingAnalysis.Instance.LastBrakingDistance;
        
        var v0 = initialSpeedKmh / 3.6f;
        var vImpact = impactSpeedKmh / 3.6f;

        var fullBrakingDistance = (v0 * v0 * brakingDistanceBeforeImpact) / (v0 * v0 - vImpact * vImpact);

        return fullBrakingDistance;
    }

    private void OnDestroy()
    {
        if (obstacle != null)
        {
            obstacle.OnSegmentEnd -= DisplayNormalResults;
        }
    }
}