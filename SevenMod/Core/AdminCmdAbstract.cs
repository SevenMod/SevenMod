// <copyright file="AdminCmdAbstract.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an admin command, a special console command that is managed by the mod, with
    /// built in permission checking.
    /// </summary>
    public abstract class AdminCmdAbstract : ConsoleCmdAbstract
    {
        /// <summary>
        /// The map of registered admin commands to their required <see cref="AdminFlags"/>.
        /// </summary>
        internal static readonly Dictionary<Type, AdminFlags> Registry = new Dictionary<Type, AdminFlags>();

        /// <inheritdoc/>
        public override int DefaultPermissionLevel => 2000;

        /// <summary>
        /// Executes the logic of the admin command. This is called after checking the calling
        /// client's permission.
        /// </summary>
        /// <param name="args">The list of arguments supplied by the client.</param>
        /// <param name="senderInfo">Information about the calling client.</param>
        public abstract void Exec(List<string> args, CommandSenderInfo senderInfo);

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (!Registry.ContainsKey(this.GetType()))
            {
                this.ReplyToCommand(senderInfo, "Unknown command");
                return;
            }

            if (senderInfo.RemoteClientInfo != null)
            {
                var flags = Registry[this.GetType()];
                if ((flags == 0) || !AdminManager.CheckAccess(senderInfo.RemoteClientInfo, flags))
                {
                    this.ReplyToCommand(senderInfo, "You do not have access to that command");
                    return;
                }
            }

            this.Exec(args, senderInfo);
        }

        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the
        /// client used to call the currently executing command.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="message">The message to send.</param>
        protected void ReplyToCommand(CommandSenderInfo senderInfo, string message)
        {
            if ((senderInfo.RemoteClientInfo != null) && ChatHook.ShouldReplyToChat(senderInfo.RemoteClientInfo))
            {
                senderInfo.RemoteClientInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, message, "[SM]", false, "SevenMod", false));
            }
            else
            {
                SdtdConsole.Instance.Output(message);
            }
        }

        /// <summary>
        /// Parse a player target string into a list of currently connected clients.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <returns>A list of <see cref="ClientInfo"/> objects representing the matching
        /// clients.</returns>
        protected List<ClientInfo> ParseTargetString(CommandSenderInfo senderInfo, string targetString)
        {
            var list = new List<ClientInfo>();

            var target = this.ParseSingleTargetString(senderInfo, targetString);
            if (target == null)
            {
                list.Add(target);
            }

            foreach (var client in list)
            {
                if (!AdminManager.CanTarget(senderInfo.RemoteClientInfo, target))
                {
                    this.ReplyToCommand(senderInfo, $"Cannot target {target.playerName}");
                    list.Remove(client);
                }
            }

            return list;
        }

        /// <summary>
        /// Parse a single player target string into a <see cref="ClientInfo"/> object representing
        /// a currently connected client, or <c>null</c> if no unique target is found.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <returns>A <see cref="ClientInfo"/> object representing a matching client if one is
        /// found; otherwise <c>null</c>.</returns>
        protected ClientInfo ParseSingleTargetString(CommandSenderInfo senderInfo, string targetString)
        {
            var count = ConsoleHelper.ParseParamPartialNameOrId(targetString, out string _, out ClientInfo target, false);
            if (count < 1 || (target == null))
            {
                this.ReplyToCommand(senderInfo, "No valid targets found");
            }
            else if (count > 1)
            {
                this.ReplyToCommand(senderInfo, "Multiple valid targets found");
            }

            return target;
        }
    }
}
