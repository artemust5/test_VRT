using UnityEngine;

namespace _ProjectRestructured.Scripts.UI
{
    public class DemoWarningDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject warningScreen;
        private void Awake()
        {
            warningScreen.SetActive(!PlayerPrefs.HasKey("LicenseKey"));
        }
    }
}
