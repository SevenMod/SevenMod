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
    public abstract class AdminCmdAbstract
    {
        /// <summary>
        /// Gets the description for the command.
        /// </summary>
        public virtual string Description { get; } = string.Empty;

        /// <summary>
        /// Executes the logic of the admin command. This is called after checking the calling
        /// client's permission.
        /// </summary>
        /// <param name="args">The list of arguments supplied by the client.</param>
        /// <param name="senderInfo">Information about the calling client.</param>
        public abstract void Execute(List<string> args, CommandSenderInfo senderInfo);

        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the
        /// client used to call the currently executing command.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="message">The message to send.</param>
        protected static void ReplyToCommand(CommandSenderInfo senderInfo, string message)
        {
            ChatHelper.ReplyToCommand(senderInfo, message);
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
                    ReplyToCommand(senderInfo, $"[SM] Cannot target {target.playerName}");
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
                ReplyToCommand(senderInfo, "[SM] No valid targets found");
            }
            else if (count > 1)
            {
                ReplyToCommand(senderInfo, "[SM] Multiple valid targets found");
            }

            return target;
        }
    }
}
