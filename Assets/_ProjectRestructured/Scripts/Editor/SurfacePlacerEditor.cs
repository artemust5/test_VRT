using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SurfacePlacerEditor
{
    static SurfacePlacerEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        // Проверяем комбинацию Ctrl + Shift + Alt + Mouse Drag
        if (e != null && e.control && e.shift && e.alt && e.type == EventType.MouseDrag && Selection.activeTransform != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Transform t = Selection.activeTransform;

                Undo.RecordObject(t, "Place on Surface");
                t.position = hit.point;

                // Поворачиваем по нормали (направление вверх совпадает с нормалью)
                // и сохраняем текущую "вперёд" ось
                Vector3 forward = Vector3.ProjectOnPlane(t.forward, hit.normal).normalized;
                if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward; // fallback

                t.rotation = Quaternion.LookRotation(forward, hit.normal);

                // Чтобы не было постоянных Raycast'ов, нужно обновить сцену
                e.Use();
                SceneView.RepaintAll();
            }
        }
    }
}
