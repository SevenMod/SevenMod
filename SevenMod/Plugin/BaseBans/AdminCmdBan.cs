// <copyright file="AdminCmdBan.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseBans
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// <para>Admin Command: sm_ban</para>
    /// <para>Bans a player from the server.</para>
    /// </summary>
    public class AdminCmdBan : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_ban" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "bans a player from the server";
        }

        /// <inheritdoc/>
        public override void Exec(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 2)
            {
                this.ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            if (!int.TryParse(args[1], out int duration) || duration < 0)
            {
                this.ReplyToCommand(senderInfo, "Invaid ban duration");
                return;
            }

            if (ConsoleHelper.ParseParamPartialNameOrId(args[0], out string playerId, out ClientInfo target) != 1)
            {
                this.ReplyToCommand(senderInfo, "No valid targets found");
                return;
            }

            if (!AdminManager.CanTarget(senderInfo.RemoteClientInfo, target))
            {
                this.ReplyToCommand(senderInfo, $"Cannot target {target.playerName}");
                return;
            }

            var unit = "minutes";
            if (duration == 0)
            {
                unit = "years";
                duration = 999999;
            }

            SdtdConsole.Instance.ExecuteSync($"ban add {playerId} {duration} {unit}", null);
        }
    }
}
