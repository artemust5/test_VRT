using System;
using System.Collections.Generic;
using _ProjectRestructured.Scripts.Data;
using TMPro;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Player {
  public class VehicleManager : MonoBehaviour {
    [Serializable]
    public class VehicleData {
      public GameObject rootObject;
      public BaseVehicleController controller;
      public Transform hudAnchor;
      public Transform xrAnchor;
      public Collider bodyCollider;
      public TextMeshProUGUI speedText;
    }

    public Transform mainHud;
    public Transform mainXr;

    public List<VehicleData> vehicles = new();

    private int currentIndex;

    public VehicleData Current => vehicles[currentIndex];

    public static VehicleManager Instance { get; private set; }

    private void Awake() {
      if (Instance == null)
        Instance = this;
      else
        Destroy(gameObject);

      currentIndex = GetCurrentVehicleIndex();
      ActivateVehicle();
    }

    public void SwitchToNext() {
      vehicles[currentIndex].rootObject.SetActive(false);
      DataManager.ChangeVehicleType();
      currentIndex = GetCurrentVehicleIndex();
      ActivateVehicle();
    }

    private void ActivateVehicle() {
      Current.rootObject.SetActive(true);
      mainHud.SetParent(Current.hudAnchor, false);
      mainXr.SetParent(Current.xrAnchor, false);
    }

    private int GetCurrentVehicleIndex() {
      return DataManager.playerData.vehicleType switch {
        VehicleType.Bicycle => 0,
        VehicleType.EScooter => 1,
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}