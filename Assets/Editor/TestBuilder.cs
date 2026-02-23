using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ZEPETO.Asset; // Namespace ini tetap wajib ada

public class TestBuilder
{
    public static void ManualConvert()
    {
        Debug.Log("🛠️ [ZEPETO BUILDER] Memulai proses konversi otomatis (SDK 3.2.12)...");

        // 1. Parsing Argumen Command Line
        string[] args = Environment.GetCommandLineArgs();
        string z_id = "";
        string z_pw = "";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-id" && i + 1 < args.Length) z_id = args[i + 1];
            if (args[i] == "-password" && i + 1 < args.Length) z_pw = args[i + 1];
        }

        // 2. Cari Prefab di Assets/InputRaw
        string inputPath = "Assets/InputRaw";
        if (!Directory.Exists(inputPath))
        {
            Debug.LogError($"❌ [BUILDER] Folder tidak ditemukan: {inputPath}");
            EditorApplication.Exit(1);
            return;
        }

        string[] prefabs = Directory.GetFiles(inputPath, "*.prefab", SearchOption.AllDirectories);
        if (prefabs.Length == 0)
        {
            Debug.LogError("❌ [BUILDER] Tidak ada file .prefab ditemukan.");
            EditorApplication.Exit(1);
            return;
        }

        // 3. Muat Prefab
        string targetPrefabPath = prefabs[0];
        GameObject targetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(targetPrefabPath);
        if (targetPrefab == null)
        {
            Debug.LogError("❌ [BUILDER] Gagal memuat prefab.");
            EditorApplication.Exit(1);
            return;
        }

        Selection.activeObject = targetPrefab;

        // 4. Proses Ekspor TANPA Opsi/Boolean
        try
        {
            var result = new StringBuilder();
            
            // Membuat statistik (Opsional, untuk log saja)
            var statistics = ZepetoAssetBundleInfo.Create(targetPrefab);
            if (statistics != null)
            {
                result.AppendLine("[ZEPETO STUDIO ARCHIVE RESULT]");
                result.AppendLine(JsonUtility.ToJson(statistics, true));
            }

            // --- BAGIAN INTI: HANYA SATU ARGUMEN ---
            // Kita panggil tanpa bool dan tanpa Option
            var packResult = ZepetoAssetPackage.Pack(targetPrefab);
            result.Append(packResult);

            Debug.Log(result.ToString());
            Debug.Log("✅ [BUILDER] Konversi selesai.");
            
            EditorApplication.Exit(0);
        }
        catch (Exception e)
        {
            Debug.LogError($"🔥 [BUILDER] Error: {e.Message}");
            EditorApplication.Exit(1);
        }
    }
}
