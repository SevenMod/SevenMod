// <copyright file="BaseCommands.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using SevenMod.Admin;
    using SevenMod.Console;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that adds the kick and who admin commands.
    /// </summary>
    public class BaseCommands : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Commands",
            Author = "SevenMod",
            Description = "Adds basic admin commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.RegAdminCmd("kick", AdminFlags.Kick, "Kicks a player from the server").Executed += this.OnKickCommandExecuted;
            this.RegAdminCmd("reloadadmins", AdminFlags.Ban, "Reloads the admin list").Executed += this.OnReloadadminsCommandExecuted;
            this.RegAdminCmd("who", AdminFlags.Generic, "Lists connected clients and their access flags").Executed += this.OnWhoCommandExecuted;
        }

        /// <summary>
        /// Called when the kick admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnKickCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            foreach (var target in this.ParseTargetString(e.SenderInfo, e.Arguments[0]))
            {
                SdtdConsole.Instance.ExecuteSync($"kick {target.playerId}", null);
            }
        }

        /// <summary>
        /// Called when the reloadadmins admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnReloadadminsCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            AdminManager.ReloadAdmins();
        }

        /// <summary>
        /// Called when the who admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnWhoCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            this.ReplyToCommand(e.SenderInfo, $"  {"Name", -24} {"Username", -18} {"Admin access"}");
            foreach (var client in GameManager.Instance.World.Players.dict.Values)
            {
                var player = ConnectionManager.Instance.Clients.ForEntityId(client.entityId);
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

                    this.ReplyToCommand(e.SenderInfo, $"  {player.playerName, -24} {player.playerId, -18} {flags}");
                }
            }
        }
    }
}
