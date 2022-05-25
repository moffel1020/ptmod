using System.Collections.Generic;
using UnityEngine;
using PTmod;

namespace MapEditor
{
    [PTmodLoad]
    public class Reference : MonoBehaviour
    {
        public static Dictionary<string, GameObject> refDict;
        public static List<string> refList;
        public GameObject reference;
        public Transform[] children;

        private void Awake()
        {
            refDict = new Dictionary<string, GameObject>();
            refList = new List<string>();
        }

        public Reference(GameObject go, string name, bool detachChildren = true)
        {
            reference = Instantiate(go);
            reference.name = name;
            reference.tag = "Obstacle";

            if(detachChildren) 
            {
                reference.transform.DetachChildren();
            }
            else
            {
                for (int i = 0; i < reference.transform.childCount; i++)
                {
                    reference.transform.GetChild(i).name = "Reference_child";
                }
            }

            DontDestroyOnLoad(this.reference);
            reference.transform.position = new Vector3(5000f, 5000f, 5000f);    //place far away so you dont see it (good programming)

            refDict.Add(name, reference);
            refList.Add(name);
            Debug.Log("added object reference " + reference.name);
        }
    }
}