using UnityEngine;

namespace _ProjectRestructured.Scripts.Gameplay.Hazard
{
    public class RccAICarDistanceLimiter : MonoBehaviour
    {
        public Transform playerCar;
        public float desiredDistance = 20f; // Positive: behind, Negative: ahead
        public bool copyPlayerSpeed;
        public float maxDistanceError = 5f; // Если ошибка больше — перестаём копировать скорость

        private RTC_CarController aiCar;
        private Rigidbody playerRb;

        void Start()
        {
            aiCar = GetComponent<RTC_CarController>();
            if (playerCar != null)
                playerRb = playerCar.GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (playerCar == null || aiCar == null || playerRb == null) return;

            Vector3 playerForward = playerCar.forward;
            Vector3 toAI = transform.position - playerCar.position;

            float signedDistance = Vector3.Dot(toAI, playerForward);
            float distanceError = signedDistance - desiredDistance;

            float targetSpeed;

            bool isDistanceAcceptable = Mathf.Abs(distanceError) < maxDistanceError;

            if (copyPlayerSpeed && isDistanceAcceptable)
            {
                float playerSpeed = playerRb.velocity.magnitude * 3.6f;
                targetSpeed = playerSpeed - distanceError * 1.5f;
            }
            else
            {
                targetSpeed = -distanceError * 5f;
            }

            targetSpeed = Mathf.Clamp(targetSpeed, 0f, 150f);

            float currentSpeed = aiCar.currentSpeed;
            float speedDelta = targetSpeed - currentSpeed;

            if (speedDelta > 1f)
            {
                aiCar.throttleInput = 1f;
                aiCar.brakeInput = 0f;
            }
            else if (speedDelta < -1f)
            {
                aiCar.throttleInput = 0f;
                aiCar.brakeInput = 0.5f;
            }
            else
            {
                aiCar.throttleInput = 0f;
                aiCar.brakeInput = 0f;
            }
        }
    }
}
