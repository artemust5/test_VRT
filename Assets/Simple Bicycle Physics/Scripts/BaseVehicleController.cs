using UnityEngine;

namespace _ProjectRestructured.Scripts.Player {
  public abstract class BaseVehicleController : MonoBehaviour {
    public abstract Rigidbody rb { get; }
    public abstract float CurrentSpeed { get; }
    public abstract bool IsBraking { get; }

    public abstract void SetCustomSteerInput(float value, float sensitivity = 10f, float gravity = 5f);
    public abstract void SetCustomAccelerationInput(float value, float sensitivity = 1f, float gravity = 5f);
    public abstract void SetCustomBrakeInput(float value);
    public abstract void ActivateAdditionalKeyboardButtons();
    public abstract void SetSteeringOnOff(bool turnOn);
  }
}