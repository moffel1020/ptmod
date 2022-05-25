using System;
using System.Collections.Generic;
using UnityEngine;

namespace PTconsole
{
    public class ConsoleCommand
    {
        public static List<ConsoleCommand>commandList;
        public string name;
        public string description;
        public string usage;
        private Action<string[]> command;
        

        public ConsoleCommand(string name, string description, Action<string[]> command, string usage = "")
        {
            this.name = name;
            this.command = command;
            this.description = description;
            this.usage = usage;
            commandList.Add(this);
            Debug.Log($"Created console command: '{this.name}'");
        }

        public ConsoleCommand(string name, Action<string[]> command)
        {
            this.name = name;
            this.command = command;
            this.description = "";
            commandList.Add(this);
            Debug.Log($"Created console command: '{this.name}'");
        }

        internal void Run(string[] Args)
        {
            command.Invoke(Args);
        }
    }
}