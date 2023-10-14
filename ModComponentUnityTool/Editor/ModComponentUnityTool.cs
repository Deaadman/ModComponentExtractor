using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;

namespace Deadman
{
    public static class ModComponentUnityTool
    {
        [MenuItem("Assets/Create .ModComponent File", false, 100)]
        public static void ProcessModComponentFolder()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Error", "No directory selected.", "OK");
                return;
            }

            if (!AssetDatabase.IsValidFolder(path))
            {
                EditorUtility.DisplayDialog("Error", "Please select a valid directory.", "OK");
                return;
            }

            try
            {
                CreateModComponentFile(path);
                UnityEngine.Debug.Log(".ModComponent File Successfully Created!");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.ToString());
            }
        }

        private static void CreateModComponentFile(string path)
        {
            string name = Path.GetFileName(path);

            if (name == "blueprints" || name == "auto-mapped" || name == "gear-spawns")
                throw new Exception($"{name} cannot be used as the name of an item pack. Place this folder into a new empty folder, and use that folder instead");

            string outputPath = $"{Path.GetDirectoryName(path)}/{name}.modcomponent";

            ZipFile.CreateFromDirectory(path, outputPath, CompressionLevel.Optimal, false);
        }
    }
}