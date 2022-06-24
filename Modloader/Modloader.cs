using System.IO;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEngine;
using HarmonyLib;

namespace Doorstop
{
    public class Loader
    {
        public static void Main(string[] args)
        {
            // wait untill game is loaded
            SceneManager.sceneLoaded += PTmod.ModLoader.StartModManager;
        }
    }
}

namespace PTmod
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class PTmodLoad : System.Attribute { }


    public class ModLoader
    {
        public static GameObject ModManager;
        public static string mod_directory;


        public static void StartModManager(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= PTmod.ModLoader.StartModManager;
            var harmony = new Harmony("com.moffel.ptmod"); 
            harmony.PatchAll();
            ModManager = new GameObject("ModManager");
            ModManager.AddComponent<Core>();
            GameObject.DontDestroyOnLoad(ModManager);
            mod_directory = Directory.GetCurrentDirectory() + "\\Mods";
            LoadAllMods();
        }
        
        public static void LoadAllMods()
        {
            foreach (string path in Directory.GetFiles(mod_directory, "*.dll"))
            {
                var asm = Assembly.LoadFile(path);
                var typesToAdd = asm.GetTypes().Where(t => t.IsDefined(typeof(PTmodLoad)) && !t.IsAbstract);    //get classes with [PTmodLoad]
                foreach (Type T in typesToAdd)
                {
                    ModManager.AddComponent(T);
                    Debug.Log("[ModLoader] added: " + T.FullName);
                }
            }
        }

        public static void DeactivateAllMods()
        {
            var mods = ModManager.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var c in mods)
            {
                if (c.ToString() == "ModManager (PTmod.ModHandler)") continue;    //this is stupid
                c.enabled = false;
                Debug.Log("Disabled mod " + c.ToString());
            }
        }

        public static void ReactivateAllMods()
        {
            var mods = ModManager.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var c in mods)
            {
                c.enabled = true;
                Debug.Log("Enabled mod " + c.ToString());
            }
        }
    }
}