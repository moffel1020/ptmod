using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using UnityEngine;
using PTmod;


namespace PTconsole
{
    [PTmodLoad]
    public class Console : MonoBehaviour
    {
        private static string consoleText;
        private static string logFilePath;

        private string input;
        private string[] input_array;
        private List<string> i_history;
        private int historyCount;
        public static bool showConsole;


        private void Awake()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US", false);
        }

        private void Start()
        {
            logFilePath = Directory.GetCurrentDirectory() + "\\log.txt";
            File.Delete(logFilePath);   //delete previous log

            i_history = new List<string>();
            ConsoleCommand.commandList = new List<ConsoleCommand>();
            commands.Register();
            Application.logMessageReceived += LogCallback;
        }


        private void Update()
        {
            //toggle console if f1 is pressed
            if (Input.GetKeyDown(KeyCode.F1)) ToggleConsole();

            //unlock mouse cursor and lock camera with left alt
            if (Input.GetKeyDown(KeyCode.LeftAlt)) Core.LockCamera();
            else if (Input.GetKeyUp(KeyCode.LeftAlt)) Core.UnlockCamera();
        }


        private void OnGUI()
        {
            //coordinate display
            if (commands.enablePosDisplay)
            {
                if (Core.player == null)
                { 
                    Core.player = FindObjectOfType<PlayerMovement>();
                }
                else
                {
                    GUI.Label(new Rect(10f, Screen.height - 50f, 500f, 20f), "x: " + Core.player.transform.position.x.ToString());
                    GUI.Label(new Rect(10f, Screen.height - 35f, 500f, 20f), "y: " + Core.player.transform.position.y.ToString());
                    GUI.Label(new Rect(10f, Screen.height - 20f, 500f, 20f), "z: " + Core.player.transform.position.z.ToString());
                }
            }

            //fps display
            if (commands.enableFpsDisplay)
            {
                GUI.Label(new Rect(10f, Screen.height - 65f, 500f, 20f), "fps: " + commands.GetFps().ToString());
            }

            //---console---
            if (!showConsole) return;
            if (input == "") historyCount = 0;

            //send input when enter is pressed
            if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown)
            {
                File.AppendAllText(logFilePath, ">" + input + "\n");
                HandleInput();
                input = "";
                historyCount = 0;
            }

            //cycle through input history with arrow keys
            if (Event.current.keyCode == KeyCode.UpArrow && Event.current.type == EventType.KeyDown)
            {
                if (historyCount < i_history.Count)
                {
                    historyCount++;
                    input = i_history[historyCount - 1];
                }
            }

            if (Event.current.keyCode == KeyCode.DownArrow && Event.current.type == EventType.KeyDown)
            {
                if (historyCount > 0)
                {
                    historyCount--;
                    input = i_history[historyCount - 1];
                }
            }

            //draw console
            GUI.TextArea(new Rect(20f, 5f, Screen.width - 40f, Screen.height - 200f), consoleText);
            input = GUI.TextField(new Rect(20f, Screen.height - 195f, Screen.width - 40f, 20f), input);
        }


        private void HandleInput()
        {
            if (input == "") return;
            input_array = input.Split(' ');

            for (int i = 0; i < input_array.Length; i++)
                input_array[i] = input_array[i].Replace("+", " ");    //use a '+' for spaces

            //add input to history
            if (i_history.Count > 50) i_history.RemoveAt(50);
            i_history.Insert(0, input);

            string command = input_array[0];
            string[] Args = input_array.Skip(1).ToArray();   //remove first element(command) from input array


            foreach (ConsoleCommand cmd in ConsoleCommand.commandList)
            {
                if(cmd.name == command)
                {
                    cmd.Run(Args);
                    return;
                }
            }

            Debug.LogError("Unknown command");
        }


        public static void Print(string text)
        {
            consoleText = text + "\n" + consoleText;
            File.AppendAllText(logFilePath, text + "\n");
        }

        public static void Clear() => consoleText = "";

        public static void ToggleConsole() => showConsole = !showConsole;


        private static void LogCallback(string condition, string stackTrace, LogType type)  //print unity logs and errors to console
        {
            if(stackTrace == "") Console.Print($"[{type}] {condition}");
            else Console.Print($"[{type}] {condition} \n {stackTrace}");
        }
    }
}