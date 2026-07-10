using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using TMPro;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Data
{
    public class LicenseChecker : MonoBehaviour
    {
        #region Config

        [SerializeField] private string spreadsheetId = "";
        [SerializeField] private string pendingSheet = "";
        [SerializeField] private string allowedSheet = "";
        [SerializeField] private TextMeshProUGUI lastLicenseNumbersText;
        [SerializeField] private GameObject checkLicensePanel;
        [SerializeField] private GameObject chooseSituationPanel;

        #endregion

        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] bool enableDebug;

        #region Private fields

        private SheetsService sheetsService;
        private const string DeviceKeyKey = "DeviceKey";
        private const string LicenseKeyKey = "LicenseKey";

        #endregion

        #region Unity

        private void Awake()
        {
            debugText.transform.parent.gameObject.SetActive(enableDebug);
            AddDebug("LicenseChecker initialized");
            
            if (!GetLicenseStatus())
            {
                AddDebug("No active license found, showing license panel");
                return;
            }
            
            AddDebug("Valid license found, starting full app");
            StartFullApp();
        }

        public void RequestLicense()
        {
            var deviceKey = GetDeviceKey();
            AddDebug($"Starting license check for device: {deviceKey}");
            StartCoroutine(CheckLicenseCoroutine(deviceKey));

            var lastThreeChars = deviceKey.Length >= 3 ? deviceKey[^3..] : deviceKey;
            lastLicenseNumbersText.text = lastThreeChars;
            AddDebug($"Displaying last 3 chars of device key: {lastThreeChars}");
        }

        #endregion

        #region License Check

        private IEnumerator CheckLicenseCoroutine(string deviceKey)
        {
            AddDebug("Starting license check coroutine");
            yield return new WaitForSeconds(1f);

            InitGoogleSheetsService();

            AddDebug($"Checking if device is allowed: {deviceKey}");
            if (IsDeviceAllowed(deviceKey))
            {
                SetLicenseStatusActivated();
                StartFullApp();
                AddDebug("✅ Device is allowed, license activated");
            }
            else
            {
                AddDebug("⚠️ Device not found in allowed list, adding to pending");
                SendDeviceKeyToPending(deviceKey);
            }
        }

        private bool GetLicenseStatus()
        {
            bool hasLicense = PlayerPrefs.HasKey(LicenseKeyKey);
            AddDebug($"Checking license status: {(hasLicense ? "Active" : "Not Active")}");
            return hasLicense;
        }

        private void SetLicenseStatusActivated()
        {
            PlayerPrefs.SetString(LicenseKeyKey, "activated");
            PlayerPrefs.Save();
            AddDebug("License status set to activated in PlayerPrefs");
        }

        #endregion

        #region Google Sheets API

        private void InitGoogleSheetsService()
        {
            var credentialAsset = Resources.Load<TextAsset>("google-credentials"); // без .json
            try
            {
                AddDebug("Loading credentials from Resources...");

                if (credentialAsset == null)
                {
                    AddDebug("❌ Failed to load credentials.json from Resources.");
                    return;
                }

                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(credentialAsset.text));
                var credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.Spreadsheets);

                sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Unity License Checker",
                });

                AddDebug("✅ Google Sheets service initialized successfully.");
            }
            catch (System.Exception ex)
            {
                AddDebug("❌ Failed to initialize Sheets service: " + ex.Message);
            }
        }

        private bool IsDeviceAllowed(string lDeviceKey)
        {
            AddDebug($"Checking allowed devices in sheet: {allowedSheet}");
            
            try
            {
                var range = $"{allowedSheet}!A:A";
                AddDebug($"Making API request to range: {range}");
                
                var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = request.Execute();

                if (response.Values == null)
                {
                    AddDebug("No data found in allowed devices sheet");
                    return false;
                }

                bool isAllowed = response.Values.Any(row => row.Count > 0 && row[0].ToString() == lDeviceKey);
                AddDebug($"Device allowed check result: {(isAllowed ? "Allowed" : "Not Allowed")}");
                
                return isAllowed;
            }
            catch (System.Exception ex)
            {
                AddDebug($"❌ Error checking allowed devices: {ex.Message}");
                return false;
            }
        }

        private void SendDeviceKeyToPending(string lDeviceKey)
        {
            AddDebug($"Attempting to add device to pending sheet: {pendingSheet}");
            
            try
            {
                var valueRange = new ValueRange
                {
                    Values = new IList<object>[] { new object[] { lDeviceKey } }
                };

                var range = $"{pendingSheet}!A:A";
                AddDebug($"Preparing to append to range: {range}");
                
                var appendRequest =
                    sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
                appendRequest.ValueInputOption =
                    SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                
                var result = appendRequest.Execute();
                AddDebug($"Device added to Pending_devices. API response: {result.Updates.UpdatedCells} cells updated");
            }
            catch (System.Exception ex)
            {
                AddDebug($"❌ Error adding device to pending: {ex.Message}");
            }
        }

        #endregion

        #region DeviceKey Generator

        private string GetDeviceKey()
        {
            if (PlayerPrefs.HasKey(DeviceKeyKey))
            {
                var existingKey = PlayerPrefs.GetString(DeviceKeyKey);
                AddDebug($"Using existing device key: {existingKey}");
                return existingKey;
            }

            var newKey = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(DeviceKeyKey, newKey);
            PlayerPrefs.Save();
            AddDebug($"Generated new device key: {newKey}");
            return newKey;
        }

        #endregion

        #region HandleAppVersion

        private void StartFullApp()
        {
            AddDebug("Starting full application version");
            checkLicensePanel.SetActive(false);
            chooseSituationPanel.SetActive(true);
        }

        #endregion

        #region Debug Utilities

        private void AddDebug(string message)
        {
            var time = System.DateTime.Now.ToString("HH:mm:ss");
            if (debugText != null)
            {
                debugText.text += $"[{time}] {message}\n";
                ScrollToBottom();
            }
            Debug.Log(message);
        }

        private void ScrollToBottom()
        {
            if (debugText != null && debugText.transform.parent != null)
            {
                var scrollRect = debugText.transform.parent.GetComponentInParent<UnityEngine.UI.ScrollRect>();
                if (scrollRect != null)
                {
                    scrollRect.verticalNormalizedPosition = 0;
                }
            }
        }

        #endregion
    }
}