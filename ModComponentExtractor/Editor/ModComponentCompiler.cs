using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using UnityEditor;

public static class ModComponentExtractorEditor
{
    [MenuItem("Assets/Process ModComponent", false, 100)]
    public static void ProcessModComponent()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("Error", "No file or directory selected.", "OK");
            return;
        }

        try
        {
            ProcessPath(path);
            UnityEngine.Debug.Log("Succeeded");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
    }

    private static void ProcessPath(string path)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);

        if (Directory.Exists(fullPath))
        {
            ProcessDirectory(fullPath);
        }
        else if (File.Exists(fullPath))
        {
            ProcessFile(fullPath);
        }
        else
        {
            throw new FileNotFoundException("Could not find file or folder", fullPath);
        }
    }

    private static void ProcessDirectory(string path)
    {
        string name = Path.GetFileName(path);
        if (name.EndsWith('\\') || name.EndsWith('/'))
            name = name.Substring(0, name.Length - 1);

        if (name == "blueprints" || name == "auto-mapped" || name == "gear-spawns")
            throw new Exception($"{name} cannot be used as the name of an item pack. Place this folder into a new empty folder, and use that folder instead");

        string parentDirectory = Path.GetDirectoryName(Path.GetFullPath(path)) ?? throw new Exception("Could not get parent directory path");
        string outputPath = Path.Combine(parentDirectory, $"{name}.modcomponent");

        FastZip fastZip = new();
        fastZip.CreateZip(outputPath, path, true, "");
    }

    private static void ProcessFile(string path)
    {
        string extension = Path.GetExtension(path);

        if (extension != ".modcomponent" && extension != ".zip")
            throw new Exception($"Program cannot be used on files with extension '{extension}'");

        string fileName = Path.GetFileNameWithoutExtension(path);
        string parentDirectory = Path.GetDirectoryName(Path.GetFullPath(path)) ?? throw new Exception("Could not get parent directory path");
        string outputDirectory = Path.Combine(parentDirectory, fileName);

        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);

        Directory.CreateDirectory(outputDirectory);

        FastZip fastZip = new();
        fastZip.ExtractZip(path, outputDirectory, "");
    }
}