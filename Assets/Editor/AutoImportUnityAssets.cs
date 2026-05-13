using UnityEditor;
using UnityEngine;
using System.IO;

public class AutoImportUnityAssets : EditorWindow
{
    private string unityAssetsRoot = "Assets/unity_assets"; // mirror your Python output root
    private bool useHDRP = true; // false = URP

    [MenuItem("Tools/AI Asset Pipeline/Auto Import")]
    public static void ShowWindow()
    {
        GetWindow<AutoImportUnityAssets>("AI Asset Auto Import");
    }

    private void OnGUI()
    {
        GUILayout.Label("AI Asset → Unity Import", EditorStyles.boldLabel);

        unityAssetsRoot = EditorGUILayout.TextField("Unity Assets Root", unityAssetsRoot);
        useHDRP = EditorGUILayout.Toggle("Use HDRP (off = URP)", useHDRP);

        if (GUILayout.Button("Process All Assets"))
        {
            ProcessAllAssets();
        }
    }

    private void ProcessAllAssets()
    {
        if (!AssetDatabase.IsValidFolder(unityAssetsRoot))
        {
            Debug.LogWarning($"Folder not found: {unityAssetsRoot}");
            return;
        }

        var assetDirs = Directory.GetDirectories(unityAssetsRoot);
        foreach (var dir in assetDirs)
        {
            var relative = dir.Replace("\\", "/");
            if (!relative.StartsWith("Assets/"))
                relative = "Assets/" + relative.Substring(relative.IndexOf("unity_assets"));

            ProcessSingleAsset(relative);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[AI Pipeline] Finished processing all assets.");
    }

    private void ProcessSingleAsset(string assetRoot)
    {
        var modelsDir = Path.Combine(assetRoot, "Models").Replace("\\", "/");
        var materialsDir = Path.Combine(assetRoot, "Materials").Replace("\\", "/");
        var prefabsDir = Path.Combine(assetRoot, "Prefabs").Replace("\\", "/");

        EnsureFolder(materialsDir);
        EnsureFolder(prefabsDir);

        var modelGuids = AssetDatabase.FindAssets("t:Model", new[] { modelsDir });
        foreach (var guid in modelGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (model == null) continue;

            var instance = PrefabUtility.InstantiatePrefab(model) as GameObject;
            if (instance == null) continue;

            try
            {
                SetupMaterials(instance, materialsDir);
                SetupCollider(instance);

                var prefabPath = Path.Combine(prefabsDir, model.name + ".prefab").Replace("\\", "/");
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                Debug.Log($"[AI Pipeline] Created prefab: {prefabPath}");
            }
            finally
            {
                Object.DestroyImmediate(instance);
            }
        }
    }

    private void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        var parts = path.Split('/');
        var current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private void SetupMaterials(GameObject go, string materialsDir)
    {
        var renderer = go.GetComponentInChildren<MeshRenderer>();
        if (renderer == null) return;

        var mats = renderer.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
        {
            var mat = mats[i];
            if (mat == null) continue;

            var newMat = CreateOrGetPipelineMaterial(mat.name, materialsDir);
            if (newMat != null)
                mats[i] = newMat;
        }
        renderer.sharedMaterials = mats;
    }

    private Material CreateOrGetPipelineMaterial(string name, string materialsDir)
    {
        var matPath = Path.Combine(materialsDir, name + ".mat").Replace("\\", "/");
        var existing = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (existing != null) return existing;

        string shaderName = useHDRP ? "HDRP/Lit" : "Universal Render Pipeline/Lit";
        var shader = Shader.Find(shaderName);
        if (shader == null)
        {
            Debug.LogWarning($"Shader not found: {shaderName}");
            return null;
        }

        var mat = new Material(shader) { name = name };

        var lower = name.ToLower();
        if (lower.Contains("wood"))
            mat.SetColor("_BaseColor", new Color(0.25f, 0.15f, 0.08f, 1f));
        else if (lower.Contains("brass"))
            mat.SetColor("_BaseColor", new Color(0.8f, 0.65f, 0.3f, 1f));
        else if (lower.Contains("red_inset") || lower.Contains("red"))
            mat.SetColor("_BaseColor", new Color(0.8f, 0.1f, 0.1f, 1f));
        else
            mat.SetColor("_BaseColor", Color.gray);

        AssetDatabase.CreateAsset(mat, matPath);
        return mat;
    }

    private void SetupCollider(GameObject go)
    {
        var existing = go.GetComponentInChildren<Collider>();
        if (existing != null) return;

        var mr = go.GetComponentInChildren<MeshRenderer>();
        if (mr == null) return;

        var target = mr.gameObject;
        var box = target.AddComponent<BoxCollider>();
        box.center = mr.bounds.center - target.transform.position;
        box.size = mr.bounds.size;
    }
}