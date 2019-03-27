// <copyright file="PlayerLog.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.PlayerLog
{
    using System;
    using System.IO;
    using SevenMod.Chat;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that logs player chat and actions to a file.
    /// </summary>
    public sealed class PlayerLog : PluginAbstract, IDisposable
    {
        /// <summary>
        /// The value of the SilentChatTrigger <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue silentChatTrigger;

        /// <summary>
        /// The value of the LogPlayerChat <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue logPlayerChat;

        /// <summary>
        /// The player log file.
        /// </summary>
        private StreamWriter log;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Player Logger",
            Author = "SevenMod",
            Description = "Logs player chat and actions to a file.",
            Version = "0.1.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.silentChatTrigger = this.FindConVar("SilentChatTrigger").Value;
            this.logPlayerChat = this.CreateConVar("LogPlayerChat", "True", "Whether to log player chat messages.").Value;

            this.AutoExecConfig(true, "PlayerLog");
        }

        /// <inheritdoc/>
        public override void OnConfigsExecuted()
        {
            if (this.log == null)
            {
                var fileName = $"player_{DateTime.Now.ToString("yyyyMMdd")}.log";
                this.log = new StreamWriter($"{SMPath.Logs}{fileName}", true);
                this.WriteLine($"Player log file session started (file \"{fileName}\") (Plugin version \"{this.Info.Version}\")");
            }

            ChatHook.ChatMessage += this.OnChatMessage;
        }

        /// <inheritdoc/>
        public override void OnUnloadPlugin()
        {
            if (this.log != null)
            {
                this.WriteLine("Log file session closed");
                this.log.Dispose();
                this.log = null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ((IDisposable)this.log).Dispose();
        }

        /// <summary>
        /// Called when a chat message is received from a client.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ChatMessageEventArgs"/> object containing the event data.</param>
        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            if (e.Message.StartsWith(this.silentChatTrigger.AsString))
            {
                return;
            }

            if (this.logPlayerChat.AsBool)
            {
                var type = string.Empty;
                switch (e.Type)
                {
                    case SMChatType.Global:
                        type = "Global";
                        break;
                    case SMChatType.Friends:
                        type = "Friends";
                        break;
                    case SMChatType.Party:
                        type = "Party";
                        break;
                    case SMChatType.Whisper:
                        if (e.RecipientEntityIds.Count > 0)
                        {
                            var recipient = ConnectionManager.Instance.Clients.ForEntityId(e.RecipientEntityIds[0]);
                            type = this.GetString("Whisper to {0:L}", null, recipient);
                        }
                        else
                        {
                            type = "Whisper";
                        }

                        break;
                }

                this.WriteLine(this.GetString("{0:L} Chat ({1:s}) \"{2:s}\"", null, e.Client, type, e.Message));
            }
        }

        /// <summary>
        /// Writes a log line.
        /// </summary>
        /// <param name="line">The line to write.</param>
        private void WriteLine(string line)
        {
            var time = DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss");
            this.log.WriteLine($"L {time}: {line}");
            this.log.Flush();
        }
    }
}
