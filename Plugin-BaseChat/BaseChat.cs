// <copyright file="BaseChat.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseChat
{
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Console;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that adds basic admin chat commands.
    /// </summary>
    public class BaseChat : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Chat Commands",
            Author = "SevenMod",
            Description = "Adds basic admin chat commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.RegAdminCmd("say", AdminFlags.Chat, "Sends a message to all players").Executed += this.OnSayCommandExecuted;
            this.RegAdminCmd("psay", AdminFlags.Chat, "Sends a message privately to one player").Executed += this.OnPsayCommandExecuted;
            this.RegAdminCmd("chat", AdminFlags.Chat, "Sends a message to all admins").Executed += this.OnChatCommandExecuted;
        }

        /// <summary>
        /// Called when the say admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnSayCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            var startIdx = 0;
            var color = Colors.Green;
            if (e.Arguments.Count > 1)
            {
                if (Colors.IsValidColorName(e.Arguments[0]))
                {
                    color = Colors.GetHexFromColorName(e.Arguments[0]);
                    startIdx++;
                }
                else if (Colors.IsValidColorHex(e.Arguments[0]))
                {
                    color = e.Arguments[0].ToUpper();
                    startIdx++;
                }
            }

            var message = string.Join(" ", e.Arguments.GetRange(startIdx, e.Arguments.Count - startIdx).ToArray());
            ChatHelper.SendToAll($"[{color}]{message}[-]", "Admin");
        }

        /// <summary>
        /// Called when the psay admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnPsayCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 2)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            var target = SMConsoleHelper.ParseSingleTargetString(e.SenderInfo, e.Arguments[0]);
            if (target == null)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Player not found");
                return;
            }

            string from;
            if (e.SenderInfo.RemoteClientInfo == null)
            {
                from = "[i](Private) [Server]";
            }
            else
            {
                from = $"[i](Private) {e.SenderInfo.RemoteClientInfo.playerName}";
            }

            var message = string.Join(" ", e.Arguments.GetRange(1, e.Arguments.Count - 1).ToArray());
            ChatHelper.SendToAll($"{message}[/i]", null, from);
        }

        /// <summary>
        /// Called when the chat admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnChatCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            string from;
            if (e.SenderInfo.RemoteClientInfo == null)
            {
                from = $"[{Colors.Cyan}](Admins) [Server]";
            }
            else
            {
                from = $"[{Colors.Cyan}](Admins) {e.SenderInfo.RemoteClientInfo.playerName}";
            }

            var message = string.Join(" ", e.Arguments.ToArray());
            message = $"{message}[-]";
            foreach (var client in ConnectionManager.Instance.Clients.List)
            {
                if (AdminManager.IsAdmin(client.playerId))
                {
                    ChatHelper.SendTo(client, message, null, from);
                }
            }
        }
    }
}
