using Deadman.ModComponent.ModManager;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Deadman.ModComponent.UserInterface
{
    public class ModManagerUserInterface : EditorWindow
    {
        private ModManagerTreeView modTreeView;
        private TreeViewState treeViewState;
        private Mod selectedMod;

        [MenuItem("Mod Component/Mod Manager")]
        public static void ShowWindow()
        {
            GetWindow<ModManagerUserInterface>("Mod Manager");
        }

        private void OnEnable()
        {
            treeViewState = new TreeViewState();
            RefreshModTreeView();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh"))
            {
                RefreshModTreeView();
            }

            if (GUILayout.Button("Export Selected as .modcomponent"))
            {
                if (selectedMod != null)
                {
                    string path = EditorUtility.SaveFilePanel("Save Mod Component", "", selectedMod.Name, "modcomponent");
                    if (!string.IsNullOrEmpty(path))
                    {
                        ModManager.ModManager.ExportModAsModComponent(selectedMod, path);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("No Mod Selected", "Please select a Mod to export.", "OK");
                }
            }

            EditorGUILayout.EndHorizontal();

            modTreeView?.OnGUI(new Rect(0, 25, position.width, position.height - 50));
        }

        private void ModSelected(Mod mod)
        {
            selectedMod = mod;
        }

        private void RefreshModTreeView()
        {
            modTreeView = new ModManagerTreeView(treeViewState, ModManager.ModManager.GetAllMods());
            modTreeView.onItemSelected += ModSelected;
        }
    }
}