using System;
using System.Collections;
using _ProjectRestructured.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace _Project._Scripts
{
    [RequireComponent(typeof(InputData))]
    public class BicycleVehicle : MonoBehaviour
    {
        private float _horizontalInput;
        private float _verticalInput;

        public TextMeshProUGUI speedText;
        public Transform handle;
        bool braking;
        Rigidbody rb;

        public Vector3 COG;

        public AudioSource asphaltSound;
        public AudioSource pedalSound;
        private Coroutine fadeSoundCoroutine;

        public float motorForce;
        [SerializeField] float brakeForce;
        private float currentBrakeForce;
        public float currentSpeed;

        float steeringAngle;
        [SerializeField] float currentSteeringAngle;
        [Range(0f, 0.1f)] [SerializeField] float speedteercontrolTime;
        [SerializeField] float maxSteeringAngle;
        [Range(0, 20)] public float turnSmoothing;
        public float xrControllerCenteredPoint = 290f;

        [SerializeField] float maxlayingAngle = 45f;
        public float targetlayingAngle;
        [Range(-40, 40)] public float layingammount;
        [Range(0.000001f, 1)] [SerializeField] float leanSmoothing;

        [SerializeField] WheelCollider frontWheel;
        [SerializeField] WheelCollider backWheel;

        [SerializeField] Transform frontWheeltransform;
        [SerializeField] Transform backWheeltransform;

        [SerializeField] TrailRenderer fronttrail;
        [SerializeField] TrailRenderer rearttrail;

        public bool frontGrounded;
        public bool rearGrounded;

        private InputData _inputData;

        [SerializeField] private InputMode inputMode = InputMode.RealBicycle;
        private const float TurnSensitivity = 0.5f;

        private bool controlsEnabled = true;

        private bool segmentStarted = false;
        private float segmentStartTime = 0f;
        private bool reactionRecorded = false;

        private Vector3 segmentStartPosition;

        private Vector3 brakingStartPosition;
        private bool isBraking = false;

        private RayCastManager _rayCastManager;
        private const int DrunkDelay = 1;

        private float ReactionTime { get; set; }
        private float ReactionDistance { get; set; }
        private float BrakingDistance { get; set; }
        private float StoppingDistance { get; set; }
        private float ObstacleDistance { get; set; }
        private float ImpairedReactionTime { get; set; }
        private float ImpairedReactionDistance { get; set; }
        private float ImpactSpeed { get; set; }

        private enum InputMode
        {
            Keyboard,
            VRControllers,
            RealBicycle
        }
        
        //nick
        public bool invertAxisValueFromLeftController;
        [SerializeField] private LeftControllerAxis leftControllerAxis = LeftControllerAxis.Y;
        private float prevLeftControllerRotationAngle;
        private Quaternion prevLeftControllerRotation;
        

        private void OnEnable()
        {
            PlayerEventController.OnGameStateChange += HandleGameStateChange;
        }

        private void OnDisable()
        {
            PlayerEventController.OnGameStateChange -= HandleGameStateChange;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            _inputData = GetComponent<InputData>();
            _rayCastManager = FindObjectOfType<RayCastManager>();
            StopEmitTrail();

            asphaltSound.volume = 0;
            pedalSound.volume = 0;
            DrivingAnalysis.Initialize();

            invertAxisValueFromLeftController = PlayerPrefs.GetInt("invertAxisValueForLeftController") == 1;
            LoadInputMode();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartSegment();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                EndSegment();
            }
        }

        private void FixedUpdate()
        {
            GetInput();
            HandleEngine();
            HandleSteering();
            UpdateWheels();
            UpdateHandle();
            LayOnTurn();
            DownPresureOnSpeed();
            EmitTrail();
            DisplayCurrentSpeed();
            UpdateCollisionData();
            ManageSounds();

            if (isBraking)
            {
                if (rb.velocity.magnitude > 0.01)
                {
                    // Check if still moving
                    BrakingDistance = Vector3.Distance(brakingStartPosition, rb.position);
                }
                else
                {
                    isBraking = false; // Stop tracking once the bicycle stops
                    StoppingDistance = ReactionDistance + BrakingDistance; // Calculate stopping distance
                    Debug.Log($"Braking Distance: {BrakingDistance}m, Stopping Distance: {StoppingDistance}m");
                }
            }
        }

        public void StartSegment()
        {
            if (segmentStarted) return;

            segmentStarted = true;
            segmentStartTime = Time.time;
            segmentStartPosition = rb.position;
            reactionRecorded = false;
            isBraking = false;
            ReactionDistance = 0f;
            BrakingDistance = 0f;
            StoppingDistance = 0f;
            Debug.Log("Segment started");
        }

        public void EndSegment()
        {
            if (!segmentStarted) return;

            segmentStarted = false;
            if (isBraking)
            {
                BrakingDistance = Vector3.Distance(brakingStartPosition, rb.position);
                isBraking = false;
            }

            StoppingDistance = ReactionDistance + BrakingDistance;
            DrivingAnalysis.Instance.RegisterSegmentResults(currentSpeed,ReactionTime, ReactionDistance, BrakingDistance,
                StoppingDistance, ObstacleDistance, ImpairedReactionTime, ImpairedReactionDistance, ImpactSpeed,0, false);
            Debug.Log(
                $"Segment ended, Reaction Time: {ReactionTime}s, Reaction Distance: {ReactionDistance}m, Braking Distance: {BrakingDistance}m, Stopping Distance: {StoppingDistance}m");
        }

        public bool IsMoving()
        {
            return rb.velocity.magnitude > 0.01f;
        }

        private void ManageSounds()
        {
            if (rb.velocity.magnitude > 0.1f && !braking)
            {
                if (!asphaltSound.isPlaying)
                {
                    asphaltSound.Play();
                    pedalSound.Play();
                }

                StartFadeIn(asphaltSound, 1.0f, 1.0f);
                StartFadeIn(pedalSound, 1.0f, 1.0f);
            }
            else if (asphaltSound.isPlaying)
            {
                StartFadeOut(asphaltSound, 1.0f);
                StartFadeOut(pedalSound, 1.0f);
            }
        }

        private IEnumerator FadeIn(AudioSource audioSource, float finalVolume, float fadeTime)
        {
            float volumeIncrease = finalVolume / fadeTime * Time.deltaTime;
            while (audioSource.volume < finalVolume)
            {
                audioSource.volume += volumeIncrease;
                yield return null;
            }

            audioSource.volume = finalVolume;
        }

        private IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
        {
            float volumeDecrease = audioSource.volume / fadeTime * Time.deltaTime;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= volumeDecrease;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = 0;
        }

        private void StartFadeIn(AudioSource audioSource, float targetVolume, float duration)
        {
            if (fadeSoundCoroutine != null) StopCoroutine(fadeSoundCoroutine);
            fadeSoundCoroutine = StartCoroutine(FadeIn(audioSource, targetVolume, duration));
        }

        private void StartFadeOut(AudioSource audioSource, float duration)
        {
            if (fadeSoundCoroutine != null) StopCoroutine(fadeSoundCoroutine);
            fadeSoundCoroutine = StartCoroutine(FadeOut(audioSource, duration));
        }

        private void DisplayCurrentSpeed()
        {
            currentSpeed = rb.velocity.magnitude * 3.6f;
            speedText.text = $"{currentSpeed:0.0}";
        }
        
        private void LoadInputMode()
        {
            inputMode = PlayerPrefs.GetInt("inputMode") switch
            {
                0 => InputMode.RealBicycle,
                1 => InputMode.VRControllers,
                _ => inputMode
            };
        }

        private void GetInput()
        {
            if (!controlsEnabled)
            {
                _horizontalInput = 0;
                _verticalInput = 0;
                braking = false;
                return;
            }
            
            switch (inputMode)
            {
                case InputMode.Keyboard:
                    _horizontalInput = Input.GetAxis("Horizontal");
                    _verticalInput = Input.GetAxis("Vertical");
                    braking = Input.GetKey(KeyCode.Space);
                    break;
                case InputMode.VRControllers:
                    _inputData.RightController.TryGetFeatureValue(CommonUsages.trigger,
                        out _verticalInput);
                    _inputData.RightController.TryGetFeatureValue(CommonUsages.grip,
                        out var brakeValue);
                    braking = brakeValue > 0.1f;

                    _inputData.RightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out var stickInput);
                    _horizontalInput = stickInput.x * TurnSensitivity;
                    break;
                case InputMode.RealBicycle:
                    CalculateHorizontalInputFromRightController();
                    CalculateVerticalInputFromLeftControllerQuaternion();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!segmentStarted || !braking || reactionRecorded) return;
            var actualReactionTime = Time.time - segmentStartTime;
            var velocity = rb.velocity;

            ReactionTime = actualReactionTime;
            ReactionDistance = Vector3.Distance(segmentStartPosition, rb.position);

            ImpactSpeed = velocity.magnitude * 3.6f;

            ImpairedReactionTime = actualReactionTime + DrunkDelay;
            ImpairedReactionDistance = velocity.magnitude * ImpairedReactionTime;

            reactionRecorded = true;
            brakingStartPosition = rb.position;
            isBraking = true;
        }
        private void CalculateHorizontalInputFromRightController()
        {
            _inputData.RightController.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation);
            var currentAngleY = rotation.eulerAngles.y;
                    
            var deltaAngle = Mathf.DeltaAngle(xrControllerCenteredPoint, currentAngleY);
            if (Mathf.Abs(deltaAngle) <= 50f)
            {
                _horizontalInput = deltaAngle / 50f * TurnSensitivity;
            }
        }
        private void CalculateVerticalInputFromLeftController()
        {
            braking = false;
            _inputData.LeftController.TryGetFeatureValue(CommonUsages.deviceRotation, out var leftRotation);
            var currentAngle  = leftControllerAxis switch
            {
                LeftControllerAxis.X => leftRotation.eulerAngles.x,
                LeftControllerAxis.Y => leftRotation.eulerAngles.y,
                LeftControllerAxis.Z => leftRotation.eulerAngles.z,
                _ => throw new ArgumentOutOfRangeException()
            };

            var angleDifference = Mathf.DeltaAngle(prevLeftControllerRotationAngle, currentAngle);
        
            var rotationValue = 0f;

            // Adjust movement direction and speed
            switch (invertAxisValueFromLeftController? -angleDifference : angleDifference)
            {
                case < -1f: //forward
                    rotationValue = Mathf.Clamp01(Mathf.Abs(angleDifference) / 5f);
                    break;
                
                case > 2f:  //backward
                    // On old bicycle we can set braking value
                    break;
            }
            
            prevLeftControllerRotationAngle = currentAngle;
            _verticalInput = rotationValue;
            
            // Braking value on grip button
            _inputData.RightController.TryGetFeatureValue(CommonUsages.grip, out var gripPressingValue);
            if (!(gripPressingValue >= 0.05f)) return;
            braking = true;
            brakeForce = gripPressingValue * 10f;
        }

        private void CalculateVerticalInputFromLeftControllerQuaternion()
        {
            braking = false;
            _inputData.LeftController.TryGetFeatureValue(CommonUsages.deviceRotation, out var currentLeftControllerRotation);

            var rotationValue = 0f;
            var angleDifference = Mathf.DeltaAngle(prevLeftControllerRotation.eulerAngles.z, currentLeftControllerRotation.eulerAngles.z);

            switch (invertAxisValueFromLeftController ? -angleDifference : angleDifference)
            {
                case < -1f:
                    rotationValue = Mathf.Clamp01(Mathf.Abs(angleDifference) / 5f);
                    break;

                case > 2f:
                    break;
            }

            prevLeftControllerRotation = currentLeftControllerRotation;
            _verticalInput = rotationValue;

            
            _inputData.RightController.TryGetFeatureValue(CommonUsages.grip, out var gripPressingValue);
            if (!(gripPressingValue >= 0.05f)) return;
            braking = true;
            brakeForce = gripPressingValue * 10f;
        }
        public void SetAxisForLeftController(LeftControllerAxis axis)
        {
            leftControllerAxis = axis;
        }
            
        public void ResetXRControllerCenteredPoint()
        {
            _inputData.RightController.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation);
            xrControllerCenteredPoint = rotation.eulerAngles.y;
        }
        private void HandleEngine()
        {
            motorForce = DataManager.CurrentMotorForce;
            if (!controlsEnabled) return;

            if (_verticalInput < 0)
            {
                var reducedMotorForce = motorForce * 0.2f;
                backWheel.motorTorque = _verticalInput * reducedMotorForce;
            }
            else
            {
                backWheel.motorTorque = _verticalInput * motorForce;
            }

            currentBrakeForce = braking ? brakeForce : 0f;
            if (braking)
            {
                ApplyBraking();
            }
            else
            {
                ReleaseBrakibg();
            }
        }

        public void DownPresureOnSpeed()
        {
            Vector3 downforce = Vector3.down;
            float downpressure;
            if (rb.velocity.magnitude > 5)
            {
                downpressure = rb.velocity.magnitude;
                rb.AddForce(downforce * downpressure, ForceMode.Force);
            }
        }

        public void ApplyBraking()
        {
            //frontWheel.brakeTorque = currentbrakeForce/2;
            frontWheel.brakeTorque = currentBrakeForce;
            backWheel.brakeTorque = currentBrakeForce;
        }

        public void ReleaseBrakibg()
        {
            frontWheel.brakeTorque = 0;
            backWheel.brakeTorque = 0;
        }

        public void SpeedSteerinReductor()
        {
            if (rb.velocity.magnitude <
                5) //We set the limiting factor for the steering thus allowing how much steer we give to the player in relation to the speed
            {
                maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 50, speedteercontrolTime);
            }

            if (rb.velocity.magnitude > 5 && rb.velocity.magnitude < 10)
            {
                maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 30, speedteercontrolTime);
            }

            if (rb.velocity.magnitude > 10 && rb.velocity.magnitude < 15)
            {
                maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 15, speedteercontrolTime);
            }

            if (rb.velocity.magnitude > 15 && rb.velocity.magnitude < 20)
            {
                maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 10, speedteercontrolTime);
            }

            if (rb.velocity.magnitude > 20)
            {
                maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 5, speedteercontrolTime);
            }
        }

        public void HandleSteering()
        {
            SpeedSteerinReductor();

            turnSmoothing = DataManager.CurrentSteeringResponsiveness;
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, maxSteeringAngle * _horizontalInput, Mathf.Lerp(0.000001f,0.5f,turnSmoothing/20f));
            frontWheel.steerAngle = currentSteeringAngle;

            //We set the target laying angle to the + or - input value of our steering 
            //We invert our input for rotating in the ocrrect axis
            targetlayingAngle = maxlayingAngle * -_horizontalInput;
        }

        private void LayOnTurn()
        {
            Vector3 currentRot = transform.rotation.eulerAngles;

            if (rb.velocity.magnitude < 1)
            {
                layingammount = Mathf.LerpAngle(layingammount, 0f, 0.05f);
                transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, layingammount);
                return;
            }

            if (currentSteeringAngle < 0.5f && currentSteeringAngle > -0.5) //We're stright
            {
                layingammount = Mathf.LerpAngle(layingammount, 0f, leanSmoothing);
            }
            else //We're turning
            {
                layingammount = Mathf.LerpAngle(layingammount, targetlayingAngle, leanSmoothing);
                rb.centerOfMass = new Vector3(rb.centerOfMass.x, COG.y, rb.centerOfMass.z);
            }

            transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, layingammount);
        }

        public void UpdateWheels()
        {
            UpdateSingleWheel(frontWheel, frontWheeltransform);
            UpdateSingleWheel(backWheel, backWheeltransform);
        }

        public void UpdateHandle()
        {
            Quaternion sethandleRot;
            sethandleRot = frontWheeltransform.rotation;
            handle.localRotation = Quaternion.Euler(handle.localRotation.eulerAngles.x, currentSteeringAngle,
                handle.localRotation.eulerAngles.z);
        }

        private void EmitTrail()
        {
            frontGrounded = frontWheel.GetGroundHit(out WheelHit Fhit);
            rearGrounded = backWheel.GetGroundHit(out WheelHit Rhit);

            if (frontGrounded)
            {
                fronttrail.emitting = true;
            }
            else
            {
                fronttrail.emitting = false;
            }

            if (rearGrounded)
            {
                rearttrail.emitting = true;
            }
            else
            {
                rearttrail.emitting = false;
            }

            //fronttrail.emitting = true;
            //rearttrail.emitting = true;
        }

        private void StopEmitTrail()
        {
            // fronttrail.emitting = false;
            // rearttrail.emitting = false;
        }

        private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
        {
            Vector3 pos;
            Quaternion rot;
            wheelCollider.GetWorldPose(out pos, out rot);
            wheelTransform.rotation = rot;
            wheelTransform.position = pos;
        }

        private void HandleGameStateChange(GameState gameState)
        {
            UpdateSpeedUIVisibility(gameState);
        }

        private void UpdateSpeedUIVisibility(GameState gameState)
        {
            speedText.gameObject.SetActive(gameState == GameState.None);
        }

        public void DisableControls()
        {
            controlsEnabled = false;
            StopPlayer();
        }

        public void EnableControls()
        {
            controlsEnabled = true;
        }

        private void StopPlayer()
        {
            // Immediately stops the player by setting velocity to zero
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Also ensure braking is applied to stop the wheels
            currentBrakeForce = brakeForce;
            ApplyBraking();
        }

        private void UpdateCollisionData()
        {
            var rayCastInfo = _rayCastManager.GetRayCastInfo();

            ObstacleDistance = rayCastInfo.CollisionDistance;
            // IsObstacleInFront = rayCastInfo.HasCollision;
        }
    }
    
    public enum LeftControllerAxis
    {
        X,
        Y,
        Z
    }
}