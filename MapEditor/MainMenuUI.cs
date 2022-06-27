using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PTmod;
using PTconsole;

namespace MapEditor
{
    [PTmodLoad]
    public class Menu : MonoBehaviour
    {
        private static string[] maps;
        public static bool isActive;
        public bool loadMapsInMultiplayer = false;
        MenuController mc;

        private void Awake()
        {
            SceneManager.sceneLoaded += ToggleMainMenu;
        }

        private void Update()
        {
            if(!isActive) return;

            if (mc == null) 
                mc = FindObjectOfType<MenuController>();

            else if( mc.PrivateMatchCanvas.GetComponent<Canvas>().enabled)
                loadMapsInMultiplayer = true;

            else loadMapsInMultiplayer = false;
        }


        private static void ToggleMainMenu(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0) 
            {
                maps = GetMapNames().ToArray();
                isActive = true;
                return;
            }

            isActive = false;
        }


        private void OnGUI()
        {
            if (!isActive || Console.showConsole) return;
            if (maps == null) maps = GetMapNames().ToArray();

            GUI.BeginGroup(new Rect(Screen.width - 110f, 10f, 100f, Screen.height - 10f));

            if (loadMapsInMultiplayer)
            {
                GUI.Box(new Rect(0f, 0f, 100f, maps.Length * 25f + 30f), "Multiplayer");

                for (int i = 0; i < maps.Length; i++)
                {
                    if (GUI.Button(new Rect(10f, 25f * i + 30f, 80f, 20f), maps[i]))
                        MultiplayerLoader.multiplayerLoader.StartGame(maps[i]);
                }
            }
            else
            {
                GUI.Box(new Rect(0f, 0f, 100f, maps.Length * 25f + 70f), "Maps");

                for (int i = 0; i < maps.Length; i++)
                {
                    if (GUI.Button(new Rect(10f, 25f * i + 30f, 80f, 20f), maps[i]))
                        StartCoroutine(SaveSystem.LoadMap(maps[i]));
                }

                if (GUI.Button(new Rect(10f, maps.Length * 25f + 40f, 80f, 20f), "new map")) StartCoroutine(SaveSystem.NewMap());
            }


            GUI.EndGroup();
        }


        public static List<string> GetMapNames()
        {
            List<string> mapList = new List<string>();
            foreach (string file in Directory.GetFiles(MapEditor.CustomMapDir))
            {
                mapList.Add(Path.GetFileNameWithoutExtension(file));
            }
            return mapList;
        }
    }
}