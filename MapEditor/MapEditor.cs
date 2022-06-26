using System.Collections;
using System.IO;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using PTconsole;
using PTmod;
using HarmonyLib;

namespace MapEditor
{
    [PTmodLoad]
    class MapEditor : MonoBehaviour
    {
        public static string CustomMapDir;

        private static Reference wall;
        private static Reference arch;
        private static Reference cube;

        private static GameObject highlightedObject;
        private static Color savedColor;
        public static Color highlightColor;
        private static int inventoryCycle = 0;

        public static float editReach = 35f;
        private bool enableEditMode;
        private bool enableCursorLock;

        private static string mapName = "";

        private void Awake()
        {
            var harmony = new Harmony("moffel.mapeditor");
            harmony.PatchAll();
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\CustomMaps");
            CustomMapDir = Directory.GetCurrentDirectory() + "\\CustomMaps";
        }

        private void Start()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);

            highlightColor = new Color(0.8f, 1f, 1f);

            Debug.Log("Loading map editor");
            commands cmds = FindObjectOfType<commands>();
            cmds.Register();

            Console.Print("Map folder: " + CustomMapDir);

            StartCoroutine(CreateRefs());
        }


        private void Update()
        {

            if (highlightedObject != null)
            {
                try { highlightedObject.GetComponent<Renderer>().material.color = savedColor; }
                catch { }
            }

            if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.E)) enableEditMode = !enableEditMode;
            if (!enableEditMode || SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0) || Console.showConsole) return;
            if (Core.player == null)  return;


            if (Input.GetKeyDown(KeyCode.Mouse0) && !Core.lockCamera)    //left mouse button to place objects
            {
                Vector3 pos = Core.player.playerCam.transform.position;
                Vector3 offset = Core.player.playerCam.transform.forward * 7;
                Vector3 objectPos = pos + offset;


                if(inventoryCycle < 0 || inventoryCycle >= Reference.refList.Count) {
                    Debug.LogError($"could not spawn object, inventory only contains {Reference.refList.Count} objects");
                } else {
                    new SaveableObject("CustomObject" + SaveSystem.sObjectList.Count, Reference.refList[inventoryCycle], objectPos, new Vector3(2f, 2f, 2f), new Vector3(0f, 0f, 0f), new Color(0.5f, 0.5f, 0.5f, 0.5f), 10);
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                enableCursorLock = !enableCursorLock;
                if (enableCursorLock) Core.LockCamera();
                else Core.UnlockCamera();
            }

            if(Input.GetKeyDown(KeyCode.Z)) CycleThroughInventory(-1);
            if(Input.GetKeyDown(KeyCode.X)) CycleThroughInventory(1);

            RaycastHit hit;
            Ray ray = new Ray(Core.player.playerCam.transform.position, Core.player.playerCam.transform.TransformDirection(Vector3.forward));
            if (enableEditMode && Core.lockCamera == false && Physics.Raycast(ray, out hit, editReach))
            {
                GameObject o = hit.collider.gameObject;

                if (highlightedObject != o || highlightedObject == null)   // if the selected object has changed
                {
                    highlightedObject = o;
                    savedColor = highlightedObject.GetComponent<Renderer>().material.color; //save the color before the highlight overwrites it
                }

                o.GetComponent<Renderer>().material.color = highlightColor;
            }
        }

        private void CycleThroughInventory(int amount)
        {
            inventoryCycle += amount;
            if (inventoryCycle >= Reference.refList.Count)
            {
                inventoryCycle = 0;
            }
            else if (inventoryCycle < 0)
            {
                inventoryCycle = Reference.refList.Count - 1;
            }
        }

        private void OnGUI()
        {
            if (Console.showConsole || !enableEditMode) return;

            if(inventoryCycle < 0 || inventoryCycle >= Reference.refList.Count) GUI.Label(new Rect(Screen.width - 200f, Screen.height - 50f, 500f, 20f), "object not in list: " + inventoryCycle);
            else GUI.Label(new Rect(Screen.width - 200f, Screen.height - 50f, 500f, 20f), "Spawn object: " + Reference.refList[inventoryCycle]);

            if(highlightedObject == null) return;
            Vector3 pos = highlightedObject.transform.position;
            Vector3 scale = highlightedObject.transform.localScale;
            Vector3 rot = highlightedObject.transform.rotation.eulerAngles;
            string name = highlightedObject.name;

            Color col = new Color(0f, 0f, 0f);
            if (highlightedObject.GetComponent<Renderer>().material.color != null) { col = highlightedObject.GetComponent<Renderer>().material.color; }
            else Debug.LogWarning($"{highlightedObject.name} does not have color");


            //buttons (right side of crosshair)
            GUI.BeginGroup(new Rect(Screen.width / 2 + 100f, Screen.height / 2 - 100f, 4000f, 4000f));
            GUI.Label(new Rect(130f, 0f, 500f, 20f), "object name: " + highlightedObject.name); //display object name
            GUI.Label(new Rect(130f, 20f, 500f, 20f), "type: " + LayerToType(highlightedObject.layer)); //display object type

            if(!Core.lockCamera) 
            {
                GUI.EndGroup();
                return;
            }

            //set name
            name = GUI.TextField(new Rect(0f, 0f, 120f, 20f), name);
            highlightedObject.name = name;

            //change type button
            if (GUI.Button(new Rect(0f, 20f, 120f, 20f), "change"))
            {
                switch (highlightedObject.layer)
                {
                    case 0:
                        highlightedObject.layer = 9;
                        break;
                    case 9:
                        highlightedObject.layer = 10;
                        break;
                    case 10:
                        highlightedObject.layer = 0;
                        break;
                    default:
                        highlightedObject.layer = 10;
                        break;
                }
            }

            //copy button
            if (GUI.Button(new Rect(0f, 45f, 120f, 20f), "copy")) CopyObject(highlightedObject);

            //delete button
            if (GUI.Button(new Rect(0f, 70f, 120f, 20f), "delete")) 
            {
                if (SaveSystem.GetSaveableObject(highlightedObject) != null) SaveSystem.GetSaveableObject(highlightedObject).Remove();
                else highlightedObject.SetActive(false);
            }
            GUI.EndGroup();


            //sliders
            GUI.BeginGroup(new Rect(Screen.width / 2 - 500f, Screen.height / 2 - 100f, 4000f, 4000f));
            //position sliders
            GUI.Label(new Rect(0f, 0f, 500f, 20f), "position: " + highlightedObject.transform.position.ToString());
            pos.x = GUI.HorizontalSlider(new Rect(0f, 15f, 400f, 20f), pos.x, -200f, 200f);
            pos.y = GUI.HorizontalSlider(new Rect(0f, 30f, 400f, 20f), pos.y, -50f, 100f);
            pos.z = GUI.HorizontalSlider(new Rect(0f, 45f, 400f, 20f), pos.z, 0f, 400f);
            highlightedObject.transform.position = new Vector3(pos.x, pos.y, pos.z);

            //scale sliders
            GUI.Label(new Rect(0f, 60f, 500f, 20f), "scale: " + highlightedObject.transform.localScale.ToString());
            scale.x = GUI.HorizontalSlider(new Rect(0f, 75f, 400f, 20f), scale.x, 0f, 75f);
            scale.y = GUI.HorizontalSlider(new Rect(0f, 90f, 400f, 20f), scale.y, 0f, 75f);
            scale.z = GUI.HorizontalSlider(new Rect(0f, 105f, 400f, 20f), scale.z, 0f, 75f);
            highlightedObject.transform.localScale = new Vector3(scale.x, scale.y, scale.z);

            //color sliders
            GUI.Label(new Rect(0f, 120f, 500f, 20f), "color: " + savedColor.ToString());
            col.r = GUI.HorizontalSlider(new Rect(0f, 135f, 400f, 20f), col.r, 0f, 1f);
            col.g = GUI.HorizontalSlider(new Rect(0f, 150f, 400f, 20f), col.g, 0f, 1f);
            col.b = GUI.HorizontalSlider(new Rect(0f, 165f, 400f, 20f), col.b, 0f, 1f);
            savedColor = highlightedObject.GetComponent<Renderer>().material.color = new Color(col.r, col.g, col.b);

            //rotation sliders
            GUI.Label(new Rect(0f, 180f, 500f, 20f), "rotation: " + highlightedObject.transform.rotation.eulerAngles.ToString());
            rot.x = GUI.HorizontalSlider(new Rect(0f, 195f, 400f, 20f), rot.x, -90f, 90f);
            rot.y = GUI.HorizontalSlider(new Rect(0f, 210f, 400f, 20f), rot.y, 0f, 359f);
            rot.z = GUI.HorizontalSlider(new Rect(0f, 235f, 400f, 20f), rot.z, 0f, 359f);
            highlightedObject.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
            GUI.EndGroup();
            
            //clear map button
            if (GUI.Button(new Rect(Screen.width - 230f, 20f, 70f, 20f), "clear map")) SaveSystem.ClearMap();

            //save button
            mapName = GUI.TextField(new Rect(Screen.width - 150f, 45f, 70f, 20f), mapName);
            if (GUI.Button(new Rect(Screen.width - 150f, 20f, 70f, 20f), "save map")) SaveSystem.SaveMap(mapName);
        }


        private static IEnumerator CreateRefs()
        {
            SceneManager.LoadScene(1);      // scene 1 is arena map                            

            yield return new WaitForEndOfFrame();   //wait for frame, gameobjects load 1 frame after scene loads

            cube = new Reference(GameObject.Find("defVertWallPlatform Variant (1)"), "ReferenceCube", false);
            wall = new Reference(GameObject.Find("Cube (1)"), "ReferenceWall");
            arch = new Reference(GameObject.Find("Arch"), "ReferenceArch");

            // return back to main menu
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene(0);
            Core.LockCamera();
            Debug.Log("Map Editor initialized");
        }


        public static string LayerToType(int layer)
        {
            switch (layer)
            {
                case 2:
                    return "ignoreRaycasts";
                case 9:
                    return "ground";
                case 10:
                    return "wallrunnable";
                default:
                    return "none";
            }
        }


        public static void CopyObject(GameObject objectToCopy)
        {
            SaveableObject so = SaveSystem.GetSaveableObject(objectToCopy);
            if(so == null)
            {
                Debug.LogError("failed to copy object, object is not bound to reference");
                return;
            }

            so.Copy("CustomObject" +  SaveSystem.sObjectList.Count);
        }
    }
}