using System;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Data
{
    [Serializable]
    public class PlayerData
    {
        public InputMode inputMode = InputMode.RealBicycle;
        public VehicleType vehicleType = VehicleType.Bicycle;
        public bool inverseAxisValue = true;
        public bool noSteeringMode;

        public float steeringResponsivenessForControllers = 5f;
        public float steeringAngleMultiplierForControllers = 10f;
        public float motorForceForControllers = 10f;
        public float brakeForceForControllers = 3f;
        
        public float steeringResponsivenessForBicycle = 3f;
        public float steeringAngleMultiplierForBicycle = 10f;
        public float motorForceForBicycle = 10f;
        public float brakeForceForBicycle = 3f;
    }
    public enum InputMode
    {
        Keyboard,
        VRControllers,
        RealBicycle,
        EScooter
    }

    public enum VehicleType
    {
        Bicycle,
        EScooter
    }

    public class DataManager : MonoBehaviour
    {
        public static PlayerData playerData;

        #region SessionData
        public static float CurrentSteeringResponsiveness
        {
            get => getSteeringResponsiveness();
            set => setSteeringResponsiveness(value);
        }
        public static float CurrentSteeringAngleMultiplier
        {
            get => getSteeringAngleMultiplier();
            set => setSteeringAngleMultiplier(value);
        }
        public static float CurrentMotorForce
        {
            get => getMotorForce();
            set => setMotorForce(value);
        }
        public static float CurrentBrakeForce
        {
            get => getBrakeForce();
            set => setBrakeForce(value);
        }

        private static Func<float> getSteeringResponsiveness;
        private static Action<float> setSteeringResponsiveness;
        private static Func<float> getSteeringAngleMultiplier;
        private static Action<float> setSteeringAngleMultiplier;
        private static Func<float> getMotorForce;
        private static Action<float> setMotorForce;
        private static Func<float> getBrakeForce;
        private static Action<float> setBrakeForce;
        
        #endregion

        public static Action onDataUpdated;
        private static string filePath;
        private const string FileName = "/playerData.json";

        private void Awake()
        {
            filePath = Application.persistentDataPath + FileName;
            LoadData();
            InitializeSessionData();
        }

        private void OnDestroy()
        {
            SaveData();
            Debug.Log($"Player Data Saved To: {filePath}");
        }

        public static void ChangeInputMode()
        {
            playerData.inputMode = playerData.inputMode switch
            {
                InputMode.RealBicycle => InputMode.VRControllers,
                InputMode.VRControllers => InputMode.EScooter,
                InputMode.EScooter => InputMode.RealBicycle,
                _ => playerData.inputMode
            };

            SaveData();
            InitializeSessionData();
            onDataUpdated?.Invoke();
        }

        public static void ChangeVehicleType()
        {
            playerData.vehicleType = playerData.vehicleType == VehicleType.Bicycle ? VehicleType.EScooter : VehicleType.Bicycle;
            SaveData();
            onDataUpdated?.Invoke();
        }

        private static void SaveData()
        {
            var json = JsonUtility.ToJson(playerData);
            System.IO.File.WriteAllText(filePath, json);
        }

        private void LoadData()
        {
            if (!System.IO.File.Exists(filePath))
            {
                playerData = new PlayerData();
                Debug.Log($"New Player Data Created: {playerData.inputMode}");
                return;
            }

            var json = System.IO.File.ReadAllText(filePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            if (playerData == null)
            {
                Debug.LogError("Player data not created!");
                playerData = new PlayerData();
            }
            Debug.Log($"Existing Player Data: {playerData.inputMode}");
        }

        private static void InitializeSessionData()
        {
            switch (playerData.inputMode)
            {
                case InputMode.VRControllers:
                    getSteeringResponsiveness = () => playerData.steeringResponsivenessForControllers;
                    setSteeringResponsiveness = value => playerData.steeringResponsivenessForControllers = value;
                    getSteeringAngleMultiplier = () => playerData.steeringAngleMultiplierForControllers;
                    setSteeringAngleMultiplier = value => playerData.steeringAngleMultiplierForControllers = value;
                    getMotorForce = () => playerData.motorForceForControllers;
                    setMotorForce = value => playerData.motorForceForControllers = value;
                    getBrakeForce = () => playerData.brakeForceForControllers;
                    setBrakeForce = value => playerData.brakeForceForControllers = value;
                    break;
                case InputMode.RealBicycle:
                    getSteeringResponsiveness = () => playerData.steeringResponsivenessForBicycle;
                    setSteeringResponsiveness = value => playerData.steeringResponsivenessForBicycle = value;
                    getSteeringAngleMultiplier = () => playerData.steeringAngleMultiplierForBicycle;
                    setSteeringAngleMultiplier = value => playerData.steeringAngleMultiplierForBicycle = value;
                    getMotorForce = () => playerData.motorForceForBicycle;
                    setMotorForce = value => playerData.motorForceForBicycle = value;
                    getBrakeForce = () => playerData.brakeForceForBicycle;
                    setBrakeForce = value => playerData.brakeForceForBicycle = value;
                    break;
                case InputMode.EScooter:
                    getSteeringResponsiveness = () => playerData.steeringResponsivenessForBicycle;
                    setSteeringResponsiveness = value => playerData.steeringResponsivenessForBicycle = value;
                    getSteeringAngleMultiplier = () => playerData.steeringAngleMultiplierForBicycle;
                    setSteeringAngleMultiplier = value => playerData.steeringAngleMultiplierForBicycle = value;
                    getMotorForce = () => playerData.motorForceForBicycle;
                    setMotorForce = value => playerData.motorForceForBicycle = value;
                    getBrakeForce = () => playerData.brakeForceForBicycle;
                    setBrakeForce = value => playerData.brakeForceForBicycle = value;
                    break;
                case InputMode.Keyboard:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}