using System.IO;
using System.Linq;
using UnityEngine;
using PTmod;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using HarmonyLib;

namespace MapEditor
{


    [HarmonyPatch(typeof(PhotonRoom), "RPC_CreatePlayer")]
    class LoadedScenePatch
    {
        public static void Postfix()
        {
            var Loader = MultiplayerLoader.multiplayerLoader;
            if (Loader.mapToLoad == null)
                return;
            
            Loader.StartCoroutine(SaveSystem.LoadMap(Loader.mapToLoad));
            Loader.mapToLoad = null;
        }
    }


    [PTmodLoad]
    public class MultiplayerLoader : MonoBehaviourPunCallbacks
    {
        public static MultiplayerLoader multiplayerLoader { get; set; }
        private MultiplayerSettings settings;
        public string mapToLoad;

        [PunRPC]
        public void RPC_SetMapName(string filename)
        {
            Debug.Log("Loading map in multiplayer");
            mapToLoad = filename;
        }

        private void StartGame(string filename)
        {
            
            settings = GameObject.FindObjectOfType<MultiplayerSettings>();
            PhotonRoom room = PhotonRoom.party;

            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Can't load map, you are not the masterclient");
                return;
            }

            settings.multiplayerScene = int.Parse(File.ReadLines($"{MapEditor.CustomMapDir}\\{filename}.txt").First()) - 1;
            room.mainMenuRefs.mapSelectionObject.GetComponent<MapSelection>().selectedMapIndex = settings.multiplayerScene;

            room.StartGame();

            photonView.RPC("RPC_SetMapName", RpcTarget.All, filename);
        }

        private void Start()
        {
            multiplayerLoader = this;
            ModLoader.ModManager.AddComponent<PhotonView>();
            
            PhotonNetwork.AllocateViewID(photonView);

            var harmony = new Harmony("moffel.multiplayer");
            harmony.PatchAll();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartGame("map1");
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("View ID: " + photonView.ViewID);
            }
        }
    }
}