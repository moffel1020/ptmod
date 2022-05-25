using UnityEngine;
using PTmod;

namespace PTconsole
{
    static class commands
    {
        public static bool enablePosDisplay;
        public static bool enableFpsDisplay;
        private static float deltaTime;

        private static ConsoleCommand showpos;
        private static ConsoleCommand showfps;
        private static ConsoleCommand clear;
        private static ConsoleCommand tpRelative;
        private static ConsoleCommand teleport;
        private static ConsoleCommand dump;
        private static ConsoleCommand printhelp;


        internal static void Register()
        {
            showpos = new ConsoleCommand("showpos", "display coordinates on screen", (string[] Args) => enablePosDisplay = !enablePosDisplay);
            showfps = new ConsoleCommand("showfps", "display fps on screen", (string[] Args) => enableFpsDisplay = !enableFpsDisplay);
            clear = new ConsoleCommand("clear", "clear all text currently in the console", (string[] Args) => Console.Clear());
            dump = new ConsoleCommand("dump", "get a list of all active gameobjects", (string[] Args) => DumpGameObjects());

            printhelp = new ConsoleCommand("help", "show help messages of commands", (string[] Args) =>
            {
                if (Args.Length == 0) PrintHelp();
                else PrintHelp(Args[0]);
            }, "help or help <command>");

            teleport = new ConsoleCommand("tp", "teleport player to specified coordinates", (string[] Args) =>
            {
                TeleportPlayer(float.Parse(Args[0]), float.Parse(Args[1]), float.Parse(Args[2]));
            }, "tp <x> <y> <z>");

            tpRelative = new ConsoleCommand("tprel", "teleport the player relative to its position", (string[] Args) =>
            {
                TeleportPlayerRel(float.Parse(Args[0]), float.Parse(Args[1]), float.Parse(Args[2]));
            }, "tprel <x> <y> <z>");
        }


        private static void PrintHelp()
        {
            foreach(ConsoleCommand command in ConsoleCommand.commandList)
            {
                    string toPrint = $"{command.name}:\t {command.description}";
                    if(command.usage != "") toPrint +=  $"\tusage: {command.usage}";
                    Console.Print(toPrint);
            }
        }


        private static void PrintHelp(string commandName)
        {
            foreach (ConsoleCommand command in ConsoleCommand.commandList)
            {
                if(command.name == commandName)
                {
                    string toPrint = $"{command.name}:\t {command.description}";
                    if(command.usage != "") toPrint +=  $"\tusage: {command.usage}";
                    Console.Print(toPrint);
                }
            }
        }


        public static void TeleportPlayer(float x, float y, float z)
        {
            if (Core.player != null && !Core.player.isOnline)
                Core.player.transform.position = new Vector3(x, y, z);
        }


        public static void TeleportPlayerRel(float x, float y, float z)
        {
            if (Core.player != null && !Core.player.isOnline)
            {
                Vector3 previous = Core.player.transform.position;
                Core.player.transform.position = new Vector3(previous.x + x, previous.y + y, previous.z + z);
            }
        }


        public static float GetFps()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            return 1.0f / deltaTime;
        }


        private static void DumpGameObjects()
        {
            GameObject[] goList = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < goList.Length; i++)
            {
                if (goList[i].activeInHierarchy)
                {
                    Console.Print($"{goList[i].name}");
                }
            }
        }
    }
}