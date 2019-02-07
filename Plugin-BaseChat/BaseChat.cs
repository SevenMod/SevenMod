// <copyright file="BaseChat.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseChat
{
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Console;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that adds basic admin chat commands.
    /// </summary>
    public class BaseChat : PluginAbstract
    {
        /// <summary>
        /// The symbol for chat command shortcuts.
        /// </summary>
        private static readonly char ChatSymbol = '@';

        /// <summary>
        /// The value of the ChatMode <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue chatMode;

        /// <summary>
        /// The say <see cref="AdminCommand"/>.
        /// </summary>
        private AdminCommand sayCommand;

        /// <summary>
        /// The psay <see cref="AdminCommand"/>.
        /// </summary>
        private AdminCommand psayCommand;

        /// <summary>
        /// The chat <see cref="AdminCommand"/>.
        /// </summary>
        private AdminCommand chatCommand;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Chat Commands",
            Author = "SevenMod",
            Description = "Adds basic admin chat commands.",
            Version = "0.1.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.LoadTranslations("BaseChat.Plugin");

            this.chatMode = this.CreateConVar("ChatMode", "True", "Allow players to send messages to admin chat.").Value;

            this.AutoExecConfig(true, "BaseChat");

            this.sayCommand = this.RegAdminCmd("say", AdminFlags.Chat, "Say Description");
            this.psayCommand = this.RegAdminCmd("psay", AdminFlags.Chat, "Psay Description");
            this.chatCommand = this.RegAdminCmd("chat", AdminFlags.Chat, "Chat Description");

            this.sayCommand.Executed += this.OnSayCommandExecuted;
            this.psayCommand.Executed += this.OnPsayCommandExecuted;
            this.chatCommand.Executed += this.OnChatCommandExecuted;

            ChatHook.ChatMessage += this.OnChatMessage;
        }

        /// <inheritdoc/>
        public override void OnUnloadPlugin()
        {
            ChatHook.ChatMessage -= this.OnChatMessage;
        }

        /// <summary>
        /// Called when a chat message is received from a client.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ChatMessageEventArgs"/> object containing the event data.</param>
        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            var startIdx = 0;
            if (e.Message[startIdx] != ChatSymbol)
            {
                return;
            }

            startIdx++;

            if (e.Type == SMChatType.Global)
            {
                if (e.Message[startIdx] != ChatSymbol)
                {
                    if (!this.sayCommand.HasAccess(e.Client))
                    {
                        return;
                    }

                    this.SendChatToAll(e.Client, e.Message.Substring(startIdx));
                    e.Handled = true;
                }

                startIdx++;

                if (e.Message[startIdx] != ChatSymbol)
                {
                    if (!this.psayCommand.HasAccess(e.Client))
                    {
                        return;
                    }

                    var message = e.Message.Trim().Substring(startIdx);
                    var breakIdx = message.IndexOf(' ');
                    if (breakIdx == -1)
                    {
                        return;
                    }

                    if (!this.ParseSingleTargetString(e.Client, message.Substring(0, breakIdx), out var target))
                    {
                        e.Handled = true;
                        return;
                    }

                    this.SendPrivateChat(e.Client, target, message.Substring(breakIdx + 1));
                    e.Handled = true;
                }
            }
            else
            {
                if (!this.chatCommand.HasAccess(e.Client) && !this.chatMode.AsBool)
                {
                    return;
                }

                this.SendChatToAdmins(e.Client, e.Message.Substring(startIdx));
                e.Handled = true;
            }
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

            var message = string.Join(" ", e.Arguments.ToArray());
            this.SendChatToAll(e.Client, message);
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

            var message = string.Join(" ", e.Arguments.GetRange(1, e.Arguments.Count - 1).ToArray());
            this.SendPrivateChat(e.Client, target, message);
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

            var message = string.Join(" ", e.Arguments.ToArray());
            this.SendChatToAdmins(e.Client, message);
        }

        /// <summary>
        /// Sends a chat message to all players.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client sending the message.</param>
        /// <param name="message">The message to send.</param>
        private void SendChatToAll(SMClient client, string message)
        {
            var startIdx = message.IndexOf(' ');
            var color = Colors.White;
            if (startIdx > -1)
            {
                var firstWord = message.Substring(0, startIdx);
                if (Colors.IsValidColorName(firstWord))
                {
                    color = Colors.GetHexFromColorName(firstWord);
                }
                else if (Colors.IsValidColorHex(firstWord))
                {
                    color = firstWord.ToUpper();
                }
                else
                {
                    startIdx = -1;
                }
            }

            startIdx++;
            message = message.Substring(startIdx);

            this.LogAction(client, null, "\"{0:L}\" triggered sm say (text {1:s})", client, message);
            this.PrintToChatAll($"[{color}]{message}[-]");
        }

        /// <summary>
        /// Sends a private message from one player to another.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client sending the message.</param>
        /// <param name="target">The <see cref="SMClient"/> object representing the client to receive the message.</param>
        /// <param name="message">The message to send.</param>
        private void SendPrivateChat(SMClient client, SMClient target, string message)
        {
            if (client != target)
            {
                this.PrintToChat(client, "Private say to", target, client, message);
            }

            this.PrintToChat(target, "Private say to", target, client, message);
        }

        /// <summary>
        /// Sends a chat message to all admins.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client sending the message.</param>
        /// <param name="message">The message to send.</param>
        private void SendChatToAdmins(SMClient client, string message)
        {
            this.LogAction(client, null, "\"{0:L}\" triggered sm chat (text {1:s})", client, message);
            var fromAdmin = this.chatCommand.HasAccess(client);
            foreach (var c in ClientHelper.List)
            {
                if (c == client || this.chatCommand.HasAccess(c))
                {
                    this.PrintToChat(c, fromAdmin ? "Chat admins" : "Chat to admins", client, message);
                }
            }
        }
    }
}
