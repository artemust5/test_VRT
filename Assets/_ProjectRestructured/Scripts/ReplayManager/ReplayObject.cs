using System.Text;
using UnityEngine;

namespace _ProjectRestructured.Scripts.ReplayManager
{
    public class ReplayableObject : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string replayID;
        
        public string ReplayID => replayID;

        private void Awake()
        {
            if (!string.IsNullOrEmpty(replayID)) return;

            replayID = GenerateRandomID();
            Debug.Log($"{gameObject.name} assigned ReplayID: {replayID}");
        }
        
        private static string GenerateRandomID()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var builder = new StringBuilder(8);
            for (var i = 0; i < 8; i++)
            {
                var index = Random.Range(0, chars.Length);
                builder.Append(chars[index]);
            }

            return builder.ToString();
        }
        public ReplayObjectData CaptureReplayData()
        {
            var data = new ReplayObjectData
            {
                position = transform.position,
                rotation = transform.rotation
            };
            return data;
        }
        
        public void ApplyReplayData(ReplayObjectData data)
        {
            transform.position = data.position;
            transform.rotation = data.rotation;
        }
    }
}