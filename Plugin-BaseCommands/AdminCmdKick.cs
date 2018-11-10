// <copyright file="AdminCmdKick.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// Admin command that kicks a player from the server.
    /// </summary>
    public class AdminCmdKick : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "kicks a player from the server";

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 1)
            {
                ReplyToCommand(senderInfo, "[SM] Not enough parameters");
                return;
            }

            foreach (var target in this.ParseTargetString(senderInfo, args[0]))
            {
                SdtdConsole.Instance.ExecuteSync($"kick {target.playerId}", null);
            }
        }
    }
}
