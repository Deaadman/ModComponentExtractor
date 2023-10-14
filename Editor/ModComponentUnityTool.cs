using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;

namespace Deadman
{
    public static class ModComponentUnityTool
    {
        public const string Version = "v1.0.0";

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

            var message = $"<b>ModComponentUnityTool {Version}</b>\n";

            try
            {
                CreateModComponentFile(path);

                message += $"<b><color=green>.ModComponent File Successfully Created!</color></b>\n";
                message += "Select this log for more info if needed.\n";
            }
            catch (Exception ex)
            {
                message += $"<b><color=red>Error:</color></b> {ex.Message}\n";
                message += "Select this log for detailed error info.\n";
                message += $"<color=red>{ex}</color>\n";
            }

            UnityEngine.Debug.Log($"{message}");
        }

        private static void CreateModComponentFile(string path)
        {
            string name = Path.GetFileName(path);

            if (name == "auto-mapped" || name == "blueprints" || name == "bundle" || name == "gear-spawns" ||  name == "localizations")
                throw new Exception($"{name} cannot be used as the name of an item pack. Place this folder into a new empty folder, and use that folder instead");

            string outputPath = $"{Path.GetDirectoryName(path)}/{name}.modcomponent";

            ZipFile.CreateFromDirectory(path, outputPath, CompressionLevel.Optimal, false);
        }
    }
}