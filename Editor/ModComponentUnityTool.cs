using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

namespace Deadman
{
    public class ModComponentUnityTool : EditorWindow
    {
        public const string Version = "v1.1.0 - Polished Productivity Update";
        private static string selectedOutputPath = "";
        private static System.IO.Compression.CompressionLevel selectedCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
        private static readonly List<string> droppedFolders = new();

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

            float labelHeight = 20;
            float neededHeight = labelHeight * droppedFolders.Count;
            float dragAreaHeight = Mathf.Max(50, neededHeight);

            var dragArea = GUILayoutUtility.GetRect(0f, dragAreaHeight, GUILayout.ExpandWidth(true));
            GUI.Box(dragArea, "");

            if (droppedFolders.Count == 0)
            {
                GUI.Label(new Rect(dragArea.x, dragArea.y + (dragArea.height - labelHeight) / 2, dragArea.width, labelHeight), "Drag & Drop Folders Here", sectionTitleStyle);
            }
            else
            {
                for (int i = 0; i < droppedFolders.Count; i++)
                {
                    string folderName = Path.GetFileName(droppedFolders[i]);
                    GUI.Label(new Rect(dragArea.x, dragArea.y + labelHeight * i, dragArea.width, labelHeight), folderName, sectionTitleStyle);
                }
            }

            HandleDragAndDrop(dragArea);

            GUILayout.BeginVertical("Box");
            if (GUILayout.Button("Clear Folders", GUILayout.Height(20)))
            {
                droppedFolders.Clear();
            }

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

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
            GUILayout.Space(10);

            if (GUILayout.Button("Create .ModComponent(s)", GUILayout.Height(40)))
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

        private void HandleDragAndDrop(Rect dropArea)
        {
            var currentEvent = Event.current;
            var eventType = currentEvent.type;

            if (!dropArea.Contains(currentEvent.mousePosition))
                return;

            switch (eventType)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            var path = AssetDatabase.GetAssetPath(draggedObject);
                            if (Directory.Exists(path))
                            {
                                if (droppedFolders.Contains(path))
                                {
                                    EditorUtility.DisplayDialog("Folder Already Added", $"The folder '{Path.GetFileName(path)}' has already been added.", "OK");
                                }
                                else
                                {
                                    droppedFolders.Add(path);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public static void ProcessModComponentFolders()
        {
            if (string.IsNullOrEmpty(selectedOutputPath))
            {
                DisplayErrorMessage("No output directory selected. Please choose an output folder.");
                return;
            }

            if (droppedFolders.Count == 0)
            {
                DisplayErrorMessage("No folders selected. Please drag and drop folders to process.");
                return;
            }

            foreach (var folderPath in droppedFolders)
            {
                if (!IsValidPath(folderPath))
                {
                    DisplayErrorMessage($"The selected path '{folderPath}' is invalid.");
                    continue;
                }

                try
                {
                    string outputPath = CreateModComponentFile(folderPath);
                    EditorUtility.DisplayDialog("Success", "ModComponent File(s) Successfully Created!", "OK");
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    DisplayErrorMessage($"Error: {ex.Message}");
                }
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

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
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