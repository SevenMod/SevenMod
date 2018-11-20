// <copyright file="AdminCmdChat.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseChat
{
    using System.Collections.Generic;
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// Admin command that sends a chat message to all admins.
    /// </summary>
    public class AdminCmdChat : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "sends a message to all admins";

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 1)
            {
                ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            string sender;
            if (senderInfo.RemoteClientInfo == null)
            {
                sender = $"[{Colors.Cyan}](Admins) [Server]";
            }
            else
            {
                sender = $"[{Colors.Cyan}](Admins) {senderInfo.RemoteClientInfo.playerName}";
            }

            var message = string.Join(" ", args.ToArray());
            message = $"{message}[-]";
            foreach (var client in ConnectionManager.Instance.Clients.List)
            {
                if (AdminManager.IsAdmin(client.playerId))
                {
                    ChatHelper.SendTo(client, message, null, sender);
                }
            }
        }
    }
}
