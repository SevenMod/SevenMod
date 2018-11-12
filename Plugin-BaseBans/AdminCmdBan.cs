// <copyright file="AdminCmdBan.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseBans
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// Admin command that bans a player from the server.
    /// </summary>
    public class AdminCmdBan : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "bans a player from the server";

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 2)
            {
                ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            if (!int.TryParse(args[1], out int duration) || duration < 0)
            {
                ReplyToCommand(senderInfo, "Invaid ban duration");
                return;
            }

            var target = this.ParseSingleTargetString(senderInfo, args[0]);
            if (target != null)
            {
                var unit = "minutes";
                if (duration == 0)
                {
                    unit = "years";
                    duration = 999999;
                }

                SdtdConsole.Instance.ExecuteSync($"ban add {target.playerId} {duration} {unit}", null);
            }
        }
    }
}
