using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

namespace Deadman
{
    public class ModComponentUnityTool : EditorWindow
    {
        public const string Version = "v1.1.0-DeveloperBuild";
        private static string selectedOutputPath = "";
        private static System.IO.Compression.CompressionLevel selectedCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;

        [MenuItem("MCUT/Tool Interface")]
        public static void ShowWindow()
        {
            GetWindow<ModComponentUnityTool>("MCUT Interface");
        }

        void OnGUI()
        {
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };

            var versionStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 16
            };

            var sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize = 14
            };

            GUILayout.Label("ModComponent Unity Tool", titleStyle);
            GUILayout.Label(Version, versionStyle);

            GUILayout.Space(10);

            GUILayout.BeginVertical("Box");
            GUILayout.Label("Configuration", sectionTitleStyle);

            selectedCompressionLevel = (System.IO.Compression.CompressionLevel)EditorGUILayout.EnumPopup("Compression Level", selectedCompressionLevel);

            GUILayout.Space(5);

            if (GUILayout.Button("Choose Output Folder"))
            {
                ChooseOutputFolder();
            }

            if (!string.IsNullOrEmpty(selectedOutputPath))
            {
                EditorGUILayout.LabelField("Selected Path: ", selectedOutputPath);
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create .ModComponent File(s)", GUILayout.Height(40)))
            {
                ProcessModComponentFolders();
            }
        }

        private static void ChooseOutputFolder()
        {
            string path = EditorUtility.OpenFolderPanel("Choose Output Folder", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                selectedOutputPath = path;
            }
        }

        public static void ProcessModComponentFolders()
        {
            UnityEngine.Object[] selectedObjects = Selection.objects;

            if (selectedObjects.Length == 0)
            {
                DisplayErrorMessage("No folders selected.");
                return;
            }

            int total = selectedObjects.Length;
            int current = 0;

            foreach (var selectedObject in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(selectedObject);

                if (!IsValidPath(path))
                {
                    DisplayErrorMessage($"The selected path '{path}' is invalid.");
                    continue;
                }

                var message = $"<b>ModComponentUnityTool {Version}</b>\n";

                try
                {
                    string outputPath = CreateModComponentFile(path);

                    message += $"<b><color=green>.ModComponent File Successfully created at {outputPath}!</color></b>\n";
                }
                catch (Exception ex)
                {
                    message += $"<b><color=red>Error:</color></b> {ex.Message}\n";
                    message += $"<color=red>{ex}</color>\n";
                }

                UnityEngine.Debug.Log(message);
            }
        }

        private static string CreateModComponentFile(string path)
        {
            string name = Path.GetFileName(path);

            if (name == "auto-mapped" || name == "blueprints" || name == "bundle" || name == "gear-spawns" || name == "localizations")
            {
                throw new Exception($"{name} cannot be used as the name of an item pack. Place this folder into a new empty folder, and use that folder instead");
            }

            string outputPath;
            if (!string.IsNullOrEmpty(selectedOutputPath))
            {
                outputPath = Path.Combine(selectedOutputPath, $"{name}.modcomponent");
            }
            else
            {
                outputPath = Path.Combine(Path.GetDirectoryName(path), $"{name}.modcomponent");
            }

            ZipFile.CreateFromDirectory(path, outputPath, selectedCompressionLevel, false);

            return outputPath;
        }

        private static void DisplayErrorMessage(string message)
        {
            EditorUtility.DisplayDialog("Error", message, "OK");
        }

        private static bool IsValidPath(string path)
        {
            return !string.IsNullOrEmpty(path) && path.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }
    }
}