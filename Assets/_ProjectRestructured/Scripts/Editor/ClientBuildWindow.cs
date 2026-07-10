using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using _Project._Scripts.Nick;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _ProjectRestructured.Scripts.Editor
{
    public class ClientBuildWindow : EditorWindow
    {
        private ClientBuildSettings buildSettings;
        private int selectedIndex;
        private string[] clientNames;

        [MenuItem("Tools/Client Build Selector")]
        public static void ShowWindow()
        {
            GetWindow<ClientBuildWindow>("Client Build Selector");
        }

        private void OnEnable()
        {
            buildSettings = AssetDatabase.FindAssets("t:ClientBuildSettings")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ClientBuildSettings>)
                .FirstOrDefault();

            if (buildSettings != null)
                clientNames = buildSettings.GetAllClientNames().ToArray();
        }

        private void OnGUI()
        {
            if (buildSettings == null)
            {
                EditorGUILayout.HelpBox("ClientBuildSettings.asset not found!", MessageType.Warning);
                return;
            }

            if (clientNames == null || clientNames.Length == 0)
            {
                EditorGUILayout.HelpBox("No clients in ClientBuildSettings list.", MessageType.Info);
                return;
            }

            selectedIndex = EditorGUILayout.Popup("Client:", selectedIndex, clientNames);

            if (GUILayout.Button("Apply in Build Settings"))
            {
                ApplyScenesToBuildSettings(clientNames[selectedIndex]);
            }
        }

        private void ApplyScenesToBuildSettings(string clientName)
        {
            var paths = buildSettings.GetScenePathsForClient(clientName);
            if (paths == null || paths.Count == 0)
            {
                Debug.LogWarning("Client doesn't have scenes selected");
                return;
            }

            // Update build settings with selected client scenes
            EditorBuildSettings.scenes = paths
                .Select(path => new EditorBuildSettingsScene(path, true))
                .ToArray();

            SyncSceneButtons();
            Debug.Log($"Build Settings updated for client: {clientName}");
        }

        private void SyncSceneButtons()
        {
            // Load the target scene by name, if not already loaded
            var targetScene = SceneManager.GetSceneByName("MainMenu");
            if (!targetScene.isLoaded)
            {
                // Use EditorSceneManager instead of SceneManager in the editor
                EditorSceneManager.OpenScene("Assets/_ProjectRestructured/Scenes/MainMenu.unity",
                    OpenSceneMode.Additive);
            }

            // Get scene names in the same order as build settings
            var scenesWithIndex = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select((scene, index) => new
                {
                    Name = System.IO.Path.GetFileNameWithoutExtension(scene.path),
                    Index = index
                })
                .ToDictionary(x => x.Name, x => x.Index);

            // Find the 'SceneList' object, even if it's inactive
            var scenesParent = GameObject.Find("SceneList");

            if (scenesParent == null)
            {
                Debug.LogError("Object 'SceneList' not found in the scene.");
                return;
            }

            // First, deactivate all scene buttons and collect references
            var childButtons = new List<Transform>();
            for (int i = 0; i < scenesParent.transform.childCount; i++)
            {
                var child = scenesParent.transform.GetChild(i);
                var sceneName = child.name;

                // Set active state based on whether it's in the build settings
                var isInBuildSettings = scenesWithIndex.ContainsKey(sceneName);
                child.gameObject.SetActive(isInBuildSettings);

                if (isInBuildSettings)
                {
                    childButtons.Add(child);
                }
            }

            // Sort the active buttons by their index in build settings
            childButtons.Sort((a, b) => scenesWithIndex[a.name].CompareTo(scenesWithIndex[b.name]));

            // Apply the new sibling indices to match build settings order
            for (var i = 0; i < childButtons.Count; i++)
            {
                childButtons[i].SetSiblingIndex(i);
            }

            // Find the 'UINavigationForSceneList' object, even if it's inactive
            var uiNavigator = GameObject.Find("UINavigationForSceneList")?.GetComponent<UINavigator>();
            if (uiNavigator != null)
            {
                // Clear existing buttons in the navigator
                var navigatorButtonsList = GetButtonsListFromNavigator(uiNavigator);
                if (navigatorButtonsList != null)
                {
                    navigatorButtonsList.Clear();

                    // Add only active buttons to the navigator in the sorted order
                    navigatorButtonsList.AddRange(childButtons.Select(button => button.GetComponent<Button>()));

                    Debug.Log($"Updated UINavigator with {navigatorButtonsList.Count} buttons");
                }
                else
                {
                    Debug.LogError("Could not access button list in UINavigator component");
                }
            }
            else
            {
                Debug.LogWarning("UINavigationForSceneList object or UINavigator component not found");
            }
            
            // Mark the MainMenu scene as dirty so changes can be saved
            var mainMenuScene = SceneManager.GetSceneByName("MainMenu");
            if (mainMenuScene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(mainMenuScene);
            }

            // Mark modified objects as dirty
            EditorUtility.SetDirty(scenesParent);

            if (uiNavigator != null)
            {
                EditorUtility.SetDirty(uiNavigator);
            }

        }

// Helper method to get the buttons list from UINavigator using reflection
        private List<Button> GetButtonsListFromNavigator(UINavigator navigator)
        {
            var buttonsField = navigator.buttons;
            return buttonsField;
        }

    }
}
