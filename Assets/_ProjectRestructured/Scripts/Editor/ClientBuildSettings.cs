using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _ProjectRestructured.Scripts.Editor
{
    [CreateAssetMenu(fileName = "AllClientBuildSettings", menuName = "Tools/Build/Client Settings")]
    public class ClientBuildSettings : ScriptableObject
    {
        [System.Serializable]
        public class ClientSettings
        {
            public string clientName;
            public List<SceneAsset> sceneAssets = new();
        }

        public List<ClientSettings> clients = new();

        public List<string> GetScenePathsForClient(string clientName)
        {
            var client = clients.FirstOrDefault(c => c.clientName == clientName);
            return client?.sceneAssets
                .Select(AssetDatabase.GetAssetPath)
                .Where(path => !string.IsNullOrEmpty(path))
                .ToList();
        }

        public List<string> GetAllClientNames() => clients.Select(c => c.clientName).ToList();
    }
}