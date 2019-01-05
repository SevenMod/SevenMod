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
            this.LoadTranslations("BaseChat.Plugin");

            this.RegAdminCmd("say", AdminFlags.Chat, "Say Description").Executed += this.OnSayCommandExecuted;
            this.RegAdminCmd("psay", AdminFlags.Chat, "Psay Description").Executed += this.OnPsayCommandExecuted;
            this.RegAdminCmd("chat", AdminFlags.Chat, "Chat Description").Executed += this.OnChatCommandExecuted;
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
                e.Command.PrintUsage(e.Client, "[{0:t}] <{1:t}>", "color", "message");
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
            this.PrintToChatAll($"[{color}]{message}[-]");
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
                e.Command.PrintUsage(e.Client, "<{0:t}> <{1:t}>", "target", "message");
                return;
            }

            if (!this.ParseSingleTargetString(e.Client, e.Arguments[0], out var target))
            {
                this.ReplyToCommand(e.Client, "Player not found");
                return;
            }

            string from;
            if (e.Client == null)
            {
                from = $"[i]({this.GetString("Private")}) [Console]";
            }
            else
            {
                from = $"[i]({this.GetString("Private")}) {e.Client.PlayerName}";
            }

            var message = string.Join(" ", e.Arguments.GetRange(1, e.Arguments.Count - 1).ToArray());
            this.PrintToChatFrom(target, from, $"{message}[/i]");
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
                e.Command.PrintUsage(e.Client, "<{0:t}>", "message");
                return;
            }

            string from;
            if (e.Client == null)
            {
                from = $"[{Colors.Cyan}]({this.GetString("Admins")}) [Console]";
            }
            else
            {
                from = $"[{Colors.Cyan}]({this.GetString("Admins")}) {e.Client.PlayerName}";
            }

            var message = string.Join(" ", e.Arguments.ToArray());
            message = $"{message}[-]";
            foreach (var client in ClientHelper.List)
            {
                if (AdminManager.IsAdmin(client.PlayerId))
                {
                    this.PrintToChatFrom(client, from, message);
                }
            }
        }
    }
}
