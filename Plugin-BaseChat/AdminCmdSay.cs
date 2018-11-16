// <copyright file="AdminCmdSay.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseChat
{
    using System.Collections.Generic;
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// Admin command that sends a chat message to all players.
    /// </summary>
    public class AdminCmdSay : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "sends a message to all players";

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 1)
            {
                ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            var startIdx = 0;
            var color = Colors.Green;
            if (args.Count > 1)
            {
                if (Colors.IsValidColorName(args[0]))
                {
                    color = Colors.GetHexFromColorName(args[0]);
                    startIdx++;
                }
                else if (Colors.IsValidColorHex(args[0]))
                {
                    color = args[0].ToUpper();
                    startIdx++;
                }
            }

            var message = string.Join(" ", args.GetRange(startIdx, args.Count - startIdx).ToArray());
            ChatHelper.SendToAll($"[{color}]{message}[-]", "Admin");
        }
    }
}
