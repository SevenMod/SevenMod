// <copyright file="AdminCmdWho.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// <para>Admin Command: sm_who</para>
    /// <para>Displays a list of connected clients and their access flags.</para>
    /// </summary>
    public class AdminCmdWho : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_who" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "lists connected clients and their access flags";
        }

        /// <inheritdoc/>
        public override void Exec(List<string> args, CommandSenderInfo senderInfo)
        {
            this.ReplyToCommand(senderInfo, string.Format("  {0,-24} {1,-18} {2}", "Name", "Username", "Admin access"));
            foreach (var client in GameManager.Instance.World.Players.dict.Values)
            {
                var player = ConnectionManager.Instance.GetClientInfoForEntityId(client.entityId);
                var admin = AdminManager.GetAdmin(player.playerId);
                if (admin != null)
                {
                    var flags = string.Empty;
                    foreach (var keyValue in AdminManager.AdminFlagKeys)
                    {
                        if ((admin.Flags & keyValue.Value) == keyValue.Value)
                        {
                            flags += keyValue.Key;
                        }
                    }

                    this.ReplyToCommand(senderInfo, string.Format("  {0,-24} {1,-18} {2}", player.playerName, player.playerId, flags));
                }
            }
        }
    }
}
