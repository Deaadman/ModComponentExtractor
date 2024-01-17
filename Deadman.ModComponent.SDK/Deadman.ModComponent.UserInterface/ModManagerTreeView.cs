using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using Deadman.ModComponent.ModManager;
using UnityEditor;
using UnityEngine;

namespace Deadman.ModComponent.UserInterface
{
    public class ModManagerTreeView : TreeView
    {
        private Dictionary<int, Mod> idToMod;
        public delegate void ItemSelectedAction(Mod mod);
        public event ItemSelectedAction onItemSelected;

        public ModManagerTreeView(TreeViewState treeViewState, List<Mod> mods)
            : base(treeViewState)
        {
            idToMod = new Dictionary<int, Mod>();
            int id = 1;
            foreach (var mod in mods)
            {
                idToMod.Add(id++, mod);
            }

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();

            foreach (var kvp in idToMod)
            {
                allItems.Add(new TreeViewItem { id = kvp.Key, depth = 0, displayName = kvp.Value.Name });
            }

            SetupParentsAndChildrenFromDepths(root, allItems);

            return root;
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            if (idToMod.TryGetValue(id, out Mod mod))
            {
                Selection.activeObject = mod;
                onItemSelected?.Invoke(mod);
            }
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (idToMod.TryGetValue(args.item.id, out Mod mod))
            {
                Rect labelRect = args.rowRect;
                labelRect.x += GetContentIndent(args.item);
                Rect iconRect = labelRect;
                iconRect.width = iconRect.height;
                iconRect.x += 2;

                Texture2D icon = EditorGUIUtility.ObjectContent(mod, typeof(Mod)).image as Texture2D;
                GUI.DrawTexture(iconRect, icon);

                labelRect.x += iconRect.width + 4;

                EditorGUI.LabelField(labelRect, args.label);
            }
            else
            {
                base.RowGUI(args);
            }
        }
    }
}