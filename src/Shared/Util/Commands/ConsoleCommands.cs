﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Shared.Util.Commands
{
    /// <summary>
    /// Console command manager
    /// </summary>
    public class ConsoleCommands : CommandManager<ConsoleCommand, ConsoleCommandFunc>
    {
        public ConsoleCommands()
        {
            Commands = new Dictionary<string, ConsoleCommand>();

            Add("help", "Displays this help", HandleHelp);
            Add("exit", "Closes application/server", HandleExit);
            Add("status", "Displays application status", HandleStatus);
            Add("sendpkt", "Sends an empty packet with the specified packet id", HandleSendPkt);
        }

        /// <summary>
        /// Adds new command handler.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="handler"></param>
        public void Add(string name, string description, ConsoleCommandFunc handler)
        {
            Add(name, "", description, handler);
        }

        /// <summary>
        /// Adds new command handler.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="usage"></param>
        /// <param name="description"></param>
        /// <param name="handler"></param>
        public void Add(string name, string usage, string description, ConsoleCommandFunc handler)
        {
            Commands[name] = new ConsoleCommand(name, usage, description, handler);
        }

        /// <summary>
        /// Waits and parses commands till "exit" is entered.
        /// </summary>
        public void Wait()
        {
            // Just wait if not running in a console
            if (!ConsoleUtil.UserInteractive)
            {
                var reset = new ManualResetEvent(false);
                reset.WaitOne();
                return;
            }

            Log.Info("Type 'help' for a list of console commands.");

            while (true)
            {
                var line = Console.ReadLine();

                var args = ParseLine(line);
                if (args.Count == 0)
                    continue;

                var command = GetCommand(args[0]);
                if (command == null)
                {
                    Log.Info("Unknown command '{0}'", args[0]);
                    continue;
                }

                var result = command.Func(line, args);
                if (result == CommandResult.Break)
                {
                    break;
                }
                else if (result == CommandResult.Fail)
                {
                    Log.Error("Failed to run command '{0}'.", command.Name);
                }
                else if (result == CommandResult.InvalidArgument)
                {
                    Log.Info("Usage: {0} {1}", command.Name, command.Usage);
                }
            }
        }

        protected virtual CommandResult HandleSendPkt(string command, IList<string> args)
        {
            return CommandResult.Fail;
        }

        protected virtual CommandResult HandleHelp(string command, IList<string> args)
        {
            var maxLength = Commands.Values.Max(a => a.Name.Length);

            Log.Info("Available commands");
            foreach (var cmd in Commands.Values.OrderBy(a => a.Name))
                Log.Info("  {0,-" + (maxLength + 2) + "}{1}", cmd.Name, cmd.Description);

            return CommandResult.Okay;
        }

        protected virtual CommandResult HandleStatus(string command, IList<string> args)
        {
            Log.Status("Memory Usage: {0:N0} KB", Math.Round(GC.GetTotalMemory(false) / 1024f));

            return CommandResult.Okay;
        }

        protected virtual CommandResult HandleExit(string command, IList<string> args)
        {
            ConsoleUtil.Exit(0, false);

            return CommandResult.Okay;
        }
    }

    public class ConsoleCommand : Command<ConsoleCommandFunc>
    {
        public ConsoleCommand(string name, string usage, string description, ConsoleCommandFunc func)
            : base(name, usage, description, func)
        {
        }
    }

    public delegate CommandResult ConsoleCommandFunc(string command, IList<string> args);
}
