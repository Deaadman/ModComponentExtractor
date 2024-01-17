using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Compression;

namespace Deadman.ModComponent.ModManager
{
    public class ModManager
    {
        private const string ModAssetsPath = "Assets/_ModComponent";

        public static List<Mod> GetAllMods()
        {
            List<Mod> mods = new();

            string[] guids = AssetDatabase.FindAssets("t:Mod", new[] { ModAssetsPath });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Mod mod = AssetDatabase.LoadAssetAtPath<Mod>(assetPath);
                if (mod != null)
                {
                    mods.Add(mod);
                }
            }

            return mods;
        }

        public static string ExportModAsModComponent(Mod mod, string outputPath)
        {
            string assetPath = AssetDatabase.GetAssetPath(mod);
            string modFolderPath = Path.GetDirectoryName(assetPath);

            string buildInfoPath = Path.Combine(modFolderPath, "BuildInfo.json");
            string json = JsonUtility.ToJson(mod, true);
            File.WriteAllText(buildInfoPath, json);

            string modComponentPath = outputPath ?? $"{mod.Name}.modcomponent";
            if (File.Exists(modComponentPath))
            {
                File.Delete(modComponentPath);
            }

            using (var zip = ZipFile.Open(modComponentPath, ZipArchiveMode.Create))
            {
                DirectoryInfo di = new(modFolderPath);
                foreach (FileInfo file in di.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (file.Extension != ".meta" && !file.FullName.Replace('\\', '/').EndsWith(assetPath))
                    {
                        string relativePath = file.FullName[(di.FullName.Length + 1)..].Replace('\\', '/');
                        zip.CreateEntryFromFile(file.FullName, relativePath, System.IO.Compression.CompressionLevel.Optimal);
                    }
                }
            }

            File.Delete(buildInfoPath);

            return modComponentPath;
        }
    }
}