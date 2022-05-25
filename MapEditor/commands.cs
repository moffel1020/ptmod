using System.IO;
using UnityEngine;
using PTconsole;
using PTmod;

namespace MapEditor
{
    [PTmodLoad]
    class commands : MonoBehaviour
    {
        private static ConsoleCommand printGoInfo;
        private static ConsoleCommand showmaps;
        private static ConsoleCommand setGoInfo;
        private static ConsoleCommand clearmap;
        private static ConsoleCommand loadmap;
        private static ConsoleCommand savemap;
        private static ConsoleCommand setHighlightColor;
        private static ConsoleCommand setEditReach;
        private static ConsoleCommand printrefs;
        private static ConsoleCommand camspeed;
        
        internal void Register()
        {
            showmaps = new ConsoleCommand("showmaps", "print all map names in CustomMaps", (string [] Args) => PrintMaps());
            clearmap = new ConsoleCommand("clearmap", "clear the arena map", (string[] Args) => SaveSystem.ClearMap());
            loadmap = new ConsoleCommand("loadmap", "load a map by name", (string[] Args) => StartCoroutine(SaveSystem.LoadMap(Args[0])), "loadmap <custom map name>");
            savemap = new ConsoleCommand("savemap", "save current map to a file with a given name", (string[] Args) => SaveSystem.SaveMap(Args[0]), "savemap <custom map name>");
            setEditReach = new ConsoleCommand("reach", "set map editor reach to edit gameobjects, default is 35", (string[] Args) => MapEditor.editReach = float.Parse(Args[0]), "reach <amount>");
            printGoInfo = new ConsoleCommand("print", "print the info of a gameobject", (string[] Args) => PrintGameObjectInfo(Args[0]), "print <gameobject name>");
            setHighlightColor = new ConsoleCommand("highlight", "change the color of map editor object highlight", (string[] Args) =>
            {
                MapEditor.highlightColor = new Color(float.Parse(Args[0]), float.Parse(Args[1]), float.Parse(Args[2]));
            }, "highlight <r> <g> <b>   (values between 0 and 1)");

            setGoInfo = new ConsoleCommand("set","change a property of a gameobject" , (string[] Args) =>
            {
                if (Args.Length == 3) ChangeGameObject(Args[0], Args[1], Args[2]);
                else if (Args.Length == 5) ChangeGameObject(Args[0], Args[1], float.Parse(Args[2]), float.Parse(Args[3]), float.Parse(Args[4])); 
            }, "set <name> <property> <value> [value] [value]");

            printrefs = new ConsoleCommand("printrefs","print object references of the map editor", (string[] Args) =>
            {
                foreach (string key in Reference.refDict.Keys)
                {
                    Console.Print(key);
                }
            });
            camspeed = new ConsoleCommand("camspeed", "set speed of freecam camera, default is 20", (string[] Args) =>
            {
                FreeCam.CamSpeed = float.Parse(Args[0]);
            }, "camspeed <speed>");
        }

        private static void PrintMaps()
        {
            foreach (string path in Directory.GetFiles(MapEditor.CustomMapDir))
            {
                Console.Print(Path.GetFileNameWithoutExtension(path));
            }
        }

        private static void PrintGameObjectInfo(string name)
        {
            GameObject go = GameObject.Find(name);
            Console.Print("");
            Console.Print($"name: {go.name} tag: {go.tag}");
            Console.Print($"position: {go.transform.position}");
            Console.Print($"scale: {go.transform.localScale}");
            Console.Print($"rotation: {go.transform.rotation.eulerAngles}");
            Console.Print($"layer: {go.layer}");
            Console.Print($"color: {go.GetComponent<Renderer>().material.color}");

            for (int i = 0; i < go.transform.childCount; ++i)
                Console.Print("child: " + go.transform.GetChild(i));

            Material[] mats = go.GetComponent<Renderer>().sharedMaterials;
            foreach (Material mat in mats)
            {
                Console.Print($"material: {mat.name}");
            }
        }

        private static void ChangeGameObject(string name, string property, float a, float b, float c)
        {
            GameObject go = GameObject.Find(name);
            switch (property)
            {
                case "scale":
                    go.transform.localScale = new Vector3(a, b, c);
                    Console.Print($"set scale to: {go.transform.localScale} for {go.name}");
                    break;

                case "position":
                    go.transform.position = new Vector3(a, b, c);
                    Console.Print($"set position to: {go.transform.position} for {go.name}");
                    break;

                case "color":
                    go.GetComponent<Renderer>().material.color = new Color(a, b, c);
                    Console.Print($"color set to {go.GetComponent<Renderer>().material.color} for {go.name}");
                    break;

                case "rotation":
                    go.transform.rotation = Quaternion.Euler(a, b, c);
                    Console.Print($"rotation set to {go.transform.rotation.eulerAngles} for {go.name}");
                    break;
            }
        }

        private static void ChangeGameObject(string name, string property, string input)
        {
            GameObject go = GameObject.Find(name);

            switch (property)
            {
                case "name":
                    go.name = input;
                    Console.Print($"set name to: {go.name}");
                    break;

                case "active":
                    Console.Print($"set {go.name} to inactive");
                    go.SetActive(bool.Parse(input));
                    break;

                case "layer":
                    go.layer = int.Parse(input);
                    Console.Print($"set layer to: {go.layer} for {go.name}");
                    break;
            }
        }
    }
}