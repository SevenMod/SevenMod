// <copyright file="AdminCommandManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System.Collections.Generic;
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// Manages admin commands.
    /// </summary>
    internal class AdminCommandManager
    {
        /// <summary>
        /// The list of registered admin commands.
        /// </summary>
        private static Dictionary<string, AdminCommand> commands = new Dictionary<string, AdminCommand>();

        /// <summary>
        /// Find an existing admin command with the specified name.
        /// </summary>
        /// <param name="command">The name of the admin command to locate.</param>
        /// <returns>The <see cref="AdminCommand"/> object representing the admin command if found; otherwise <c>null</c>.</returns>
        public static AdminCommand FindCommand(string command)
        {
            commands.TryGetValue(command.Trim().ToLower(), out AdminCommand adminCommand);
            return adminCommand;
        }

        /// <summary>
        /// Creates a new <see cref="AdminCommand"/> or returns the existing one if one with the same name already exists.
        /// </summary>
        /// <param name="plugin">The plugin creating the admin command.</param>
        /// <param name="command">The name of the admin command.</param>
        /// <param name="accessFlags">The <see cref="AdminFlags"/> value required to execute the admin command.</param>
        /// <param name="description">An optional description for the admin command.</param>
        /// <returns>The <see cref="AdminCommand"/> object representing the admin command.</returns>
        public static AdminCommand CreateAdminCommand(IPluginAPI plugin, string command, AdminFlags accessFlags, string description = "")
        {
            command = command.Trim();

            AdminCommand adminCommand;
            var key = command.ToLower();
            if (commands.ContainsKey(key))
            {
                adminCommand = commands[key];
            }
            else
            {
                adminCommand = new AdminCommand(plugin, command, description, accessFlags);
                commands[key] = adminCommand;
            }

            return adminCommand;
        }

        /// <summary>
        /// Executes an admin command.
        /// </summary>
        /// <param name="command">The name of the admin command to execute.</param>
        /// <param name="arguments">The arguments for the admin command.</param>
        /// <param name="senderInfo">The <see cref="CommandSenderInfo"/> object representing the client executing the command.</param>
        public static void ExecuteCommand(string command, List<string> arguments, CommandSenderInfo senderInfo)
        {
            var key = command.Trim().ToLower();
            if (!commands.ContainsKey(key))
            {
                ChatHelper.ReplyToCommand(senderInfo, "Unknown command");
                return;
            }

            var info = commands[key];

            if (senderInfo.RemoteClientInfo != null)
            {
                var flags = info.AccessFlags;
                if ((flags == 0) || !AdminManager.CheckAccess(senderInfo.RemoteClientInfo, flags))
                {
                    ChatHelper.ReplyToCommand(senderInfo, "You do not have access to that command");
                    return;
                }
            }

            info.OnExecute(arguments, senderInfo);
        }

        /// <summary>
        /// Unloads all admin commands associated with a plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which to unload admin commands.</param>
        public static void UnloadPlugin(IPluginAPI plugin)
        {
            commands.RemoveAll((AdminCommand command) => command.Plugin.Equals(plugin));
        }
    }
}
