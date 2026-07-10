using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace _ProjectRestructured.Scripts.CustomDebugs
{
    public class InternetChecker : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private string testUrl = "https://www.google.com";

        [Obsolete("Obsolete")]
        private void Start()
        {
            StartCoroutine(CheckInternetConnection());
        }

        [Obsolete("Obsolete")]
        private IEnumerator CheckInternetConnection()
        {
            ClearDebug();
            AddDebug("Starting internet connection test");
            AddDebug($"Test URL: {testUrl}");

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                AddDebug("ERROR: No network connection available");
                yield break;
            }

            AddDebug($"Connection type: {GetConnectionType()}");

            using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
            {
                request.timeout = 10;
                AddDebug("Sending request...");

                yield return request.SendWebRequest();

                AddDebug("Request completed.");
                AddDebug($"Request.result: {request.result}");
                AddDebug($"Response code: {request.responseCode}");

                if (request.result == UnityWebRequest.Result.Success)
                {
                    AddDebug("SUCCESS: Server is reachable");
                    AddDebug($"Data length: {request.downloadHandler.data?.Length ?? 0} bytes");
                }
                else
                {
                    AddDebug($"ERROR: {request.error}");
                    AddDebug($"IsNetworkError: {request.isNetworkError}");
                    AddDebug($"IsHttpError: {request.isHttpError}");
                    AddDebug($"Response text: {request.downloadHandler?.text}");
                }
            }

            AddDebug("Test complete");
        }


        private string GetConnectionType()
        {
            return Application.internetReachability switch
            {
                NetworkReachability.ReachableViaLocalAreaNetwork => "WiFi/Ethernet",
                NetworkReachability.ReachableViaCarrierDataNetwork => "Mobile data",
                _ => "Unknown connection type"
            };
        }

        private void AddDebug(string message)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            if (debugText != null)
            {
                debugText.text += $"[{time}] {message}\n";
                ScrollToBottom();
            }
            Debug.Log(message);
        }

        private void ClearDebug()
        {
            if (debugText != null)
            {
                debugText.text = "";
            }
        }

        private void ScrollToBottom()
        {
            if (debugText == null) return;
            Canvas.ForceUpdateCanvases();
            debugText.rectTransform.anchoredPosition = Vector2.zero;
        }

        [Obsolete("Obsolete")]
        public void RetryConnection()
        {
            StartCoroutine(CheckInternetConnection());
        }
    }
}