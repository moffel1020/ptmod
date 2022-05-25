using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using PTconsole;
using PTmod;


namespace MapEditor
{
    public class SaveableObject
    {
        public GameObject go;
        public string reference;


        public SaveableObject(GameObject go, string name, Vector3 position ,string reference)
        {
            this.go = GameObject.Instantiate(go);
            this.go.name = name;
            this.go.transform.position = position;
            this.reference = reference;

            SaveSystem.sObjectList.Add(this);
            Debug.Log($"spawned: {name} at: {go.transform.position}");
        }

        public SaveableObject(string name, string reference, Vector3 position, Vector3 scale, Vector3 rotation, Color color, int layer)
        {
            this.reference = reference;
            go = GameObject.Instantiate(Reference.refDict[reference]);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;
            go.transform.rotation = Quaternion.Euler(rotation);
            go.GetComponent<Renderer>().material.color = color;
            go.layer = layer;
            
            SaveSystem.sObjectList.Add(this);
            Debug.Log($"spawned: {name} at: {go.transform.position}");
        }

        public SaveableObject Copy(string name)
        {
            Vector3 pos = new Vector3(this.go.transform.position.x + 10f, this.go.transform.position.y, this.go.transform.position.z);
            return new SaveableObject(this.go, name, pos, this.reference);
        }

        public void Remove()
        {
            SaveSystem.sObjectList.Remove(this);
            go.SetActive(false);
            go = null;
            reference = null;
        }
    }


    [PTmodLoad]
    public class SaveSystem : MonoBehaviour
    {

        public static List<SaveableObject> sObjectList;

        private void Awake()
        {
            sObjectList = new List<SaveableObject>();
        }


        public static IEnumerator LoadMap(string filename)
        {
            if (!File.Exists(MapEditor.CustomMapDir + "\\" + filename + ".txt"))
            {
                Debug.LogError($"File {MapEditor.CustomMapDir}\\{filename}.txt does not exist");
                yield break;
            }

            Debug.Log("Loading map " + filename);
            SceneManager.LoadScene(1);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Core.UnlockCamera();
            ClearMap();

            string[] file = File.ReadAllLines($"{MapEditor.CustomMapDir}\\{filename}.txt");

            foreach (string line in file)
            {
                string[] lineArr = line.Split(' ');

                string name = lineArr[0];
                string reference = lineArr[1];
                Vector3 position = new Vector3(float.Parse(lineArr[2]), float.Parse(lineArr[3]), float.Parse(lineArr[4]));
                Vector3 scale = new Vector3(float.Parse(lineArr[5]), float.Parse(lineArr[6]), float.Parse(lineArr[7]));
                Vector3 rotation = new Vector3(float.Parse(lineArr[8]), float.Parse(lineArr[9]), float.Parse(lineArr[10]));
                Color color = new Color(float.Parse(lineArr[11]), float.Parse(lineArr[12]), float.Parse(lineArr[13]), float.Parse(lineArr[14]));
                int layer = int.Parse(lineArr[15]);

                new SaveableObject(name, reference, position, scale, rotation, color, layer);
            }
        }
        
        public static void SaveMap(string filename)
        {

            string filepath = $"{MapEditor.CustomMapDir}\\{filename}.txt";
            List<string> toWrite = new List<string>();

            foreach(SaveableObject o in sObjectList)
            {
                string name = o.go.name;
                string reference = o.reference;
                Vector3 position = o.go.transform.position;
                Vector3 scale = o.go.transform.localScale;
                Vector3 rotation = o.go.transform.rotation.eulerAngles;
                Color color = o.go.GetComponent<Renderer>().material.color;
                int layer = o.go.layer;
               
                toWrite.Add(
                        $"{name} " +
                        $"{reference} " +
                        $"{position.x} {position.y} {position.z} " +
                        $"{scale.x} {scale.y} {scale.z} " +
                        $"{rotation.x} {rotation.y} {rotation.z} " +
                        $"{color.r} {color.g} {color.b} {color.a} " +
                        $"{layer}"
                    );
            }

            File.WriteAllLines(filepath, toWrite);
            Debug.Log($"map saved to {MapEditor.CustomMapDir}\\{filename}.txt");
        }

        public static IEnumerator NewMap()
        {
            SceneManager.LoadScene(1);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Core.UnlockCamera();
            ClearMap();
            sObjectList.Clear();
        }


        public static void ClearMap()
        {
            sObjectList.Clear();
            GameObject[] goList = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject go in goList)
            {
                if (Reference.refDict.ContainsKey(go.name) || go.name.StartsWith("Reference")) continue;

                if (go.activeInHierarchy && go.tag == "Obstacle")
                    go.SetActive(false);
            }

            // delete stuff that doesnt have the obstacle tag for some reason
            string[] toDelete = { "Cube", "Icosphere", "def", "Small", "stadium", "sphere", "Jumbo", "Arch", "strut", "TimePanel", "floor " };

            foreach (GameObject go in goList)
            {
                if (Reference.refDict.ContainsKey(go.name) || go.name.StartsWith("Reference")) continue;

                foreach (string obj in toDelete)
                {
                    if (go.name.StartsWith(obj))
                        go.SetActive(false);
                }
            }

            Debug.Log("Cleared map");
        }


        public static SaveableObject GetSaveableObject(GameObject go)
        {
            foreach(SaveableObject so in sObjectList)
            {
                if(go == so.go)
                {
                    return so;
                }
            }

            return null;
        }
    }
}
