// <copyright file="AdminCmdPsay.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseChat
{
    using System.Collections.Generic;
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// Admin command that sends a chat message privately to one player.
    /// </summary>
    public class AdminCmdPsay : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "sends a message privately to one player";

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 2)
            {
                ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            var target = this.ParseSingleTargetString(senderInfo, args[0]);
            if (target == null)
            {
                ChatHelper.ReplyToCommand(senderInfo, "Player not found");
                return;
            }

            string sender;
            if (senderInfo.RemoteClientInfo == null)
            {
                sender = "[i](Private) [Server]";
            }
            else
            {
                sender = $"[i](Private) {senderInfo.RemoteClientInfo.playerName}";
            }

            var message = string.Join(" ", args.GetRange(1, args.Count - 1).ToArray());
            ChatHelper.SendToAll($"{message}[/i]", null, sender);
        }
    }
}
