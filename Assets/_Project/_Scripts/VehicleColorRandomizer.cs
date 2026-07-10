using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project._Scripts
{
    public class VehicleColorRandomizer : MonoBehaviour
    {
        [System.Serializable]
        public class CarMaterialGroup
        {
            public string groupName;
            public GameObject[] cars;
            public Material[] materials;
        }

        public bool showColorStatistics = true;
        public CarMaterialGroup[] groups;

        private readonly Dictionary<Material, int> _colorUsageCount = new();

        private void Awake()
        {
            foreach (var group in groups)
            {
                InitializeColorUsageCount(group.materials);

                foreach (var car in group.cars)
                {
                    var randomMaterial = SelectMaterialWithLeastUsage(group.materials);
                    IncrementMaterialUsage(randomMaterial);

                    var bodyTransform = FindTransformByName(car.transform, "Body");
                    if (bodyTransform == null) continue;

                    foreach (Transform child in bodyTransform)
                    {
                        if (!child.name.StartsWith("body_LOD")) continue;

                        var meshRenderer = child.GetComponent<MeshRenderer>();
                        if (meshRenderer == null || meshRenderer.materials.Length < 1) continue;

                        var materials = meshRenderer.materials;
                        materials[0] = randomMaterial;
                        meshRenderer.materials = materials;
                    }
                }
            }

            if (showColorStatistics)
            {
                ShowColorStatistics();
            }
        }

        private void InitializeColorUsageCount(Material[] materials)
        {
            foreach (var material in materials)
            {
                _colorUsageCount.TryAdd(material, 0);
            }
        }

        private Material SelectMaterialWithLeastUsage(Material[] materials)
        {
            var leastUsedMaterials = materials
                .Where(m => _colorUsageCount.ContainsKey(m) && _colorUsageCount[m] == _colorUsageCount.Values.Min())
                .ToArray();

            if (leastUsedMaterials.Length == 0) // Fallback in case of mismatch or error
                return materials[Random.Range(0, materials.Length)];

            return leastUsedMaterials[Random.Range(0, leastUsedMaterials.Length)];
        }

        private void IncrementMaterialUsage(Material material)
        {
            if (_colorUsageCount.ContainsKey(material))
            {
                _colorUsageCount[material]++;
            }
        }

        private void ShowColorStatistics()
        {
            Debug.Log("Color Usage Statistics:");
            foreach (var entry in _colorUsageCount)
            {
                Debug.Log($"Material: {entry.Key.name}, Usage Count: {entry.Value}");
            }
        }

        private static Transform FindTransformByName(Transform parent, string name)
        {
            if (parent.name == name) return parent;

            return parent.Cast<Transform>()
                .Select(child => FindTransformByName(child, name))
                .FirstOrDefault(result => result != null);
        }
    }
}