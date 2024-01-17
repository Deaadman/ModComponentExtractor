using UnityEngine;
using UnityEditor;
using Deadman.ModComponent.ModManager;

namespace Deadman.ModComponent
{
    [CustomEditor(typeof(Mod))]
    public class ModEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Mod mod = (Mod)target;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Name", mod.Name);
            EditorGUILayout.TextField("Author", mod.Author);
            EditorGUI.EndDisabledGroup();

            mod.Version = EditorGUILayout.TextField("Version", mod.Version);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(mod);
            }
        }
    }
}