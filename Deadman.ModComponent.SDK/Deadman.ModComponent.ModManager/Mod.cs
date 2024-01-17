using UnityEngine;

namespace Deadman.ModComponent.ModManager
{
    public class Mod : ScriptableObject
    {
        public string Name;
        public string Author;
        public string Version;

        public static Mod CreateMod(string name, string author)
        {
            Mod mod = CreateInstance<Mod>();
            mod.Name = name;
            mod.Author = author;
            return mod;
        }
    }
}