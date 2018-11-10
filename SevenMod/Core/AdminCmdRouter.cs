// <copyright file="AdminCmdRouter.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Routes the sm console command to the appropriate admin command.
    /// </summary>
    public class AdminCmdRouter : ConsoleCmdAbstract
    {
        /// <summary>
        /// The registered admin commands keyed by their command name.
        /// </summary>
        private static Dictionary<string, CommandInfo> registry = new Dictionary<string, CommandInfo>();

        /// <inheritdoc/>
        public override int DefaultPermissionLevel => 2000;

        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "accesses SevenMod admin commands";
        }

        /// <inheritdoc/>
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            if ((_params.Count < 1) || !registry.ContainsKey(_params[0]))
            {
                AdminCmdAbstract.ReplyToCommand(_senderInfo, "[SM] Unknown command");
                return;
            }

            var info = registry[_params[0]];

            if (_senderInfo.RemoteClientInfo != null)
            {
                var flags = info.AccessFlags;
                if ((flags == 0) || !AdminManager.CheckAccess(_senderInfo.RemoteClientInfo, flags))
                {
                    AdminCmdAbstract.ReplyToCommand(_senderInfo, "[SM] You do not have access to that command");
                    return;
                }
            }

            info.Handler.Execute(_params.GetRange(1, _params.Count - 1), _senderInfo);
        }

        /// <summary>
        /// Registers an admin command.
        /// </summary>
        /// <param name="plugin">The plugin registering the command.</param>
        /// <param name="command">The name of the command.</param>
        /// <param name="handler">An instance of <see cref="AdminCmdAbstract"/> which will handle
        /// calls to the command.</param>
        /// <param name="accessFlags">The <see cref="AdminFlags"/> required to execute the
        /// command.</param>
        internal static void RegisterAdminCmd(PluginAbstract plugin, string command, AdminCmdAbstract handler, AdminFlags accessFlags)
        {
            if (registry.ContainsKey(command))
            {
                return;
            }

            var info = new CommandInfo(plugin, command, handler, accessFlags);
            registry.Add(command, info);
        }

        /// <summary>
        /// Unregisters all admin commands associated with a plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        internal static void UnregisterPlugin(PluginAbstract plugin)
        {
            registry.RemoveAll((CommandInfo info) => { return plugin == info.Plugin; });
        }

        /// <summary>
        /// Represents an admin command in the registry.
        /// </summary>
        private struct CommandInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandInfo"/> struct.
            /// </summary>
            /// <param name="plugin">The plugin associated with this command.</param>
            /// <param name="command">The name of this command.</param>
            /// <param name="handler">The <see cref="AdminCmdAbstract"/> object handling this
            /// command.</param>
            /// <param name="accessFlags">The <see cref="AdminFlags"/> required to execute this
            /// command.</param>
            public CommandInfo(PluginAbstract plugin, string command, AdminCmdAbstract handler, AdminFlags accessFlags)
            {
                this.Plugin = plugin;
                this.Command = command;
                this.Handler = handler;
                this.AccessFlags = accessFlags;
            }

            /// <summary>
            /// Gets the plugin associated with this command.
            /// </summary>
            public PluginAbstract Plugin { get; }

            /// <summary>
            /// Gets the name of this command.
            /// </summary>
            public string Command { get; }

            /// <summary>
            /// Gets the <see cref="AdminCmdAbstract"/> object handling this command.
            /// </summary>
            public AdminCmdAbstract Handler { get; }

            /// <summary>
            /// Gets the <see cref="AdminFlags"/> required to execute this command.
            /// </summary>
            public AdminFlags AccessFlags { get; }
        }
    }
}
