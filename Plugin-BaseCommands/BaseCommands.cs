// <copyright file="BaseCommands.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Console;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: BaseCommands</para>
    /// <para>Adds the kick and who admin commands.</para>
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
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            this.RegAdminCmd("kick", AdminFlags.Kick, "Kicks a player from the server").Executed += this.KickExecuted;
            this.RegAdminCmd("reloadadmins", AdminFlags.Ban, "Reloads the admin list").Executed += this.ReloadAdminsExecuted;
            this.RegAdminCmd("who", AdminFlags.Generic, "Lists connected clients and their access flags").Executed += this.WhoExecuted;
        }

        /// <summary>
        /// Called when the kick admin command is executed.
        /// </summary>
        /// <param name="sender">The origin of the event.</param>
        /// <param name="e">A <see cref="AdminCommandEventArgs"/> object that contains the event
        /// data.</param>
        private void KickExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            foreach (var target in SMConsoleHelper.ParseTargetString(e.SenderInfo, e.Arguments[0]))
            {
                SdtdConsole.Instance.ExecuteSync($"kick {target.playerId}", null);
            }
        }

        /// <summary>
        /// Called when the reloadadmins admin command is executed.
        /// </summary>
        /// <param name="sender">The origin of the event.</param>
        /// <param name="e">A <see cref="AdminCommandEventArgs"/> object that contains the event
        /// data.</param>
        private void ReloadAdminsExecuted(object sender, AdminCommandEventArgs e)
        {
            AdminManager.RemoveAllAdmins();
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.ReloadAdmins();
            }
        }

        /// <summary>
        /// Called when the who admin command is executed.
        /// </summary>
        /// <param name="sender">The origin of the event.</param>
        /// <param name="e">A <see cref="AdminCommandEventArgs"/> object that contains the event
        /// data.</param>
        private void WhoExecuted(object sender, AdminCommandEventArgs e)
        {
            ChatHelper.ReplyToCommand(e.SenderInfo, string.Format("  {0,-24} {1,-18} {2}", "Name", "Username", "Admin access"));
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

                    ChatHelper.ReplyToCommand(e.SenderInfo, string.Format("  {0,-24} {1,-18} {2}", player.playerName, player.playerId, flags));
                }
            }
        }
    }
}
