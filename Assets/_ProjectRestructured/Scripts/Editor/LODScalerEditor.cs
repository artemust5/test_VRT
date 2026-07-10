using UnityEngine;
using UnityEditor;

public class LODScalerEditor : EditorWindow
{
    private float scaleFactor = 10f; // Процент изменения
    private bool applyToAllScenes = false;

    [MenuItem("Tools/Scale LOD Distances")]
    public static void ShowWindow()
    {
        GetWindow<LODScalerEditor>("Scale LOD Distances");
    }

    private void OnGUI()
    {
        GUILayout.Label("LOD Distance Scaler", EditorStyles.boldLabel);

        scaleFactor = EditorGUILayout.FloatField("Scale Factor (%)", scaleFactor);
        applyToAllScenes = EditorGUILayout.Toggle("Include Inactive Objects", applyToAllScenes);

        if (GUILayout.Button("Scale LODs"))
        {
            ScaleLODs(scaleFactor / 100f, applyToAllScenes);
        }
    }

    private static void ScaleLODs(float scaleFactor, bool includeInactive)
    {
        // Находим все LODGroup с учётом неактивных объектов
        var lodGroups = FindObjectsOfType<LODGroup>(includeInactive);

        int modifiedCount = 0;
        foreach (var lodGroup in lodGroups)
        {
            LOD[] lods = lodGroup.GetLODs();
            bool changed = false;

            for (int i = 0; i < lods.Length; i++)
            {
                // Изменяем каждую границу на указанный процент
                float newValue = lods[i].screenRelativeTransitionHeight * (1 + scaleFactor);
                lods[i].screenRelativeTransitionHeight = Mathf.Clamp01(newValue);
                changed = true;
            }

            if (changed)
            {
                lodGroup.SetLODs(lods);
                EditorUtility.SetDirty(lodGroup); // Помечаем объект как "грязный"
                modifiedCount++;
            }
        }

        Debug.Log($"✅ Modified {modifiedCount} LODGroups. Scale Factor: {scaleFactor * 100}%");
    }
}