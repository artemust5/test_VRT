using UnityEngine;
using VehiclePhysics;

namespace _ProjectRestructured.Scripts.Player {
  [RequireComponent(typeof(VPVehicleController))]
  public class VPPVehicleAdapter : BaseVehicleController {
    private VPVehicleController vppController;
    private Rigidbody targetRigidbody;
    private bool isBrakingInternal;
    private bool isSteeringEnabled = true;

    public override Rigidbody rb => targetRigidbody;
    public override float CurrentSpeed => targetRigidbody.velocity.magnitude * 3.6f;
    public override bool IsBraking => isBrakingInternal;

    private void Awake() {
      vppController = GetComponent<VPVehicleController>();
      targetRigidbody = GetComponent<Rigidbody>();
    }

    public override void SetCustomSteerInput(float value, float sensitivity, float gravity) {
      if (!isSteeringEnabled) value = 0f;
      int steerValue = Mathf.RoundToInt(Mathf.Clamp(value, -1f, 1f) * 10000);
      vppController.data.Set(Channel.Input, InputData.Steer, steerValue);
    }

    public override void SetCustomAccelerationInput(float value, float sensitivity, float gravity) {
      int throttleValue = Mathf.RoundToInt(Mathf.Clamp01(value) * 10000);
      vppController.data.Set(Channel.Input, InputData.Throttle, throttleValue);
    }

    public override void SetCustomBrakeInput(float value) {
      int brakeValue = Mathf.RoundToInt(Mathf.Clamp01(value) * 10000);
      vppController.data.Set(Channel.Input, InputData.Brake, brakeValue);
      isBrakingInternal = value > 0.1f;
    }

    public override void ActivateAdditionalKeyboardButtons() {
    }

    public override void SetSteeringOnOff(bool turnOn) {
      isSteeringEnabled = turnOn;
    }
  }
}