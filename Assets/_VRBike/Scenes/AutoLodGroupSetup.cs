using UnityEngine;

[ExecuteInEditMode]
public class AutoLODGroupSetup : MonoBehaviour
{
    [Header("Настройки LOD")]
    [Range(0, 1)] public float[] lodScreenPercentages = { 0.6f, 0.3f, 0.05f };
    [Range(0, 1)] public float cullPercentage = 0.01f; // Процент, при котором объект исчезает (кастомный LOD)

    private void OnValidate()
    {
        SetupLODGroups();
    }

    private void SetupLODGroups()
    {
        // Получить все MeshRenderer'ы в дочерних объектах
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject obj = renderer.gameObject;

            // Удалить старый LODGroup, если он существует
            LODGroup existingLODGroup = obj.GetComponent<LODGroup>();
            if (existingLODGroup != null)
            {
                DestroyImmediate(existingLODGroup);
            }

            // Создать новый LODGroup
            LODGroup lodGroup = obj.AddComponent<LODGroup>();

            // Настроить уровни LOD
            LOD[] lods = new LOD[lodScreenPercentages.Length + 1];
            for (int i = 0; i < lodScreenPercentages.Length; i++)
            {
                lods[i] = new LOD(lodScreenPercentages[i], new Renderer[] { renderer });
            }

            // Добавить "Cull" уровень, где объект исчезает
            lods[lods.Length - 1] = new LOD(cullPercentage, new Renderer[0]);

            // Применить настройки
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
    }
}