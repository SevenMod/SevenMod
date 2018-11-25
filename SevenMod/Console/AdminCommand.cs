// <copyright file="AdminCommand.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System.Collections.Generic;
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// Represents a command that can be executed by an admin user via the sm console command.
    /// </summary>
    public class AdminCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminCommand"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that created this command.</param>
        /// <param name="command">The name of the command.</param>
        /// <param name="description">The description for the command.</param>
        /// <param name="accessFlags">The <see cref="AccessFlags"/> value required to execute this
        /// command.</param>
        internal AdminCommand(PluginAbstract plugin, string command, string description, AdminFlags accessFlags)
        {
            this.Command = command;
            this.Description = description;
            this.AccessFlags = accessFlags;
            this.Plugin = plugin;
        }

        /// <summary>
        /// Handler for the <see cref="Executed"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        public delegate void AdminCommandEventHandler(object sender, AdminCommandEventArgs e);

        /// <summary>
        /// Occurs when this command is executed.
        /// </summary>
        public event AdminCommandEventHandler Executed;

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the description for the command.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the <see cref="AdminFlags"/> value required to execute this command.
        /// </summary>
        public AdminFlags AccessFlags { get; }

        /// <summary>
        /// Gets the plugin that created this command.
        /// </summary>
        internal PluginAbstract Plugin { get; }

        /// <summary>
        /// Executes the admin command.
        /// </summary>
        /// <param name="arguments">The list of arguments for the command.</param>
        /// <param name="senderInfo">The <see cref="CommandSenderInfo"/> object representing the client that is executing the command.</param>
        internal void Execute(List<string> arguments, CommandSenderInfo senderInfo)
        {
            this.Executed?.Invoke(this, new AdminCommandEventArgs(this, arguments, senderInfo));
        }
    }
}
