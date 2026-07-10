using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts : EditorWindow
{
    [MenuItem("Tools/Remove Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<RemoveMissingScripts>("Remove Missing Scripts");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Remove Missing Scripts from Scene"))
        {
            RemoveMissingScriptsFromScene();
        }
    }

    private static void RemoveMissingScriptsFromScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        int count = 0;
        foreach (GameObject go in allObjects)
        {
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        }

        Debug.Log($"Removed {count} missing scripts from the scene.");
    }
}