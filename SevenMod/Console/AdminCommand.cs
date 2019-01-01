// <copyright file="AdminCommand.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System;
    using System.Collections.Generic;
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Core;
    using SevenMod.Lang;
    using SevenMod.Logging;

    /// <summary>
    /// Represents a command that can be executed by an admin user via the sm console command.
    /// </summary>
    public class AdminCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminCommand"/> class.
        /// </summary>
        /// <param name="command">The name of the command.</param>
        /// <param name="description">The description for the command.</param>
        /// <param name="accessFlags">The <see cref="AccessFlags"/> value required to execute this
        /// command.</param>
        internal AdminCommand(string command, string description, AdminFlags accessFlags)
        {
            this.Command = command;
            this.Description = description;
            this.AccessFlags = accessFlags;
        }

        /// <summary>
        /// Occurs when this command is executed.
        /// </summary>
        public event EventHandler<AdminCommandEventArgs> Executed;

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
        /// Print the usage of the command to a client.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client.</param>
        /// <param name="usage">The parameter string for the command.</param>
        /// <param name="args">The format arguments for <paramref name="usage"/>.</param>
        public void PrintUsage(SMClient client, string usage, params object[] args)
        {
            if (ChatHook.ShouldReplyToChat(client?.ClientInfo))
            {
                ChatHelper.SendTo(client.ClientInfo, null, "{0:t}: /{1:s} {2:s}", "Usage", this.Command, Language.GetString(usage, client.ClientInfo, args));
            }
            else
            {
                SdtdConsole.Instance.Output("[SM] " + Language.GetString("{0:t}: sm {1:s} {2:s}", client?.ClientInfo, "Usage", this.Command, Language.GetString(usage, client?.ClientInfo, args)));
            }
        }

        /// <summary>
        /// Checks whether a specified client has access to this command.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client to check.</param>
        /// <returns><c>true</c> if <paramref name="client"/> has access to this command; otherwise <c>false</c>.</returns>
        public bool HasAccess(SMClient client)
        {
            return AdminCommandManager.HasAccess(client?.ClientInfo, this);
        }

        /// <summary>
        /// Executes the admin command.
        /// </summary>
        /// <param name="arguments">The list of arguments for the command.</param>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that is executing the command.</param>
        internal void OnExecute(List<string> arguments, ClientInfo client)
        {
            if (this.Executed != null)
            {
                var args = new AdminCommandEventArgs(this, arguments, client);
                foreach (EventHandler<AdminCommandEventArgs> d in this.Executed.GetInvocationList())
                {
                    try
                    {
                        d.Invoke(this, args);
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        if (d.Target is IPlugin p)
                        {
                            p.Container.SetFailState(e);
                        }
                        else
                        {
                            SMLog.Error(e);
                        }
                    }
                }
            }
        }
    }
}
