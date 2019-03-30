// <copyright file="PlayerLog.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.PlayerLog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Timers;
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
        /// The value of the LogPlayerKills <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue logPlayerKills;

        /// <summary>
        /// The player log file.
        /// </summary>
        private StreamWriter log;

        /// <summary>
        /// The timer to check for player kills.
        /// </summary>
        private Timer killCheckTimer;

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
            this.logPlayerKills = this.CreateConVar("LogPlayerKills", "True", "Whether to log player kills.").Value;

            this.AutoExecConfig(true, "PlayerLog");

            if (this.log == null)
            {
                var fileName = $"player_{DateTime.Now.ToString("yyyyMMdd")}.log";
                this.log = new StreamWriter($"{SMPath.Logs}{fileName}", true);
                this.WriteLine($"Player log file session started (file \"{fileName}\") (Plugin version \"{this.Info.Version}\")");
            }

            ChatHook.ChatMessage += this.OnChatMessage;

            this.logPlayerKills.ConVar.ValueChanged += this.OnLogPlayerKillsChanged;
        }

        /// <inheritdoc/>
        public override void OnConfigsExecuted()
        {
            if (this.logPlayerKills.AsBool)
            {
                this.killCheckTimer = new Timer(1000);
                this.killCheckTimer.Elapsed += this.OnKillCheckTimerElapsed;
                this.killCheckTimer.Start();
            }
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

            if (this.killCheckTimer != null)
            {
                this.killCheckTimer.Stop();
                this.killCheckTimer.Dispose();
                this.killCheckTimer = null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ((IDisposable)this.log).Dispose();
            ((IDisposable)this.killCheckTimer).Dispose();
        }

        /// <summary>
        /// Called when the LogPlayerKills <see cref="ConVar"/> changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ConVarChangedEventArgs"/> object containing the event data.</param>
        private void OnLogPlayerKillsChanged(object sender, ConVarChangedEventArgs e)
        {
            if (this.logPlayerKills.AsBool && this.killCheckTimer == null)
            {
                this.killCheckTimer = new Timer(1000);
                this.killCheckTimer.Elapsed += this.OnKillCheckTimerElapsed;
                this.killCheckTimer.Start();
            }
            else if (!this.logPlayerKills.AsBool && this.killCheckTimer != null)
            {
                this.killCheckTimer.Stop();
                this.killCheckTimer.Dispose();
                this.killCheckTimer = null;
            }
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

                if (e.RecipientEntityIds != null)
                {
                    var names = new List<string>();
                    foreach (var i in e.RecipientEntityIds)
                    {
                        names.Add(this.GetString("{0:L}", SMClient.Console, ConnectionManager.Instance.Clients.ForEntityId(i)));
                    }

                    this.WriteLine(string.Join(", ", names.ToArray()));
                }

                this.WriteLine("{0:L} Chat ({1:s}) \"{2:s}\"", e.Client, type, e.Message);
            }
        }

        /// <summary>
        /// Called by the <see cref="killCheckTimer"/> to check for player kills.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ElapsedEventArgs"/> object containing the event data.</param>
        private void OnKillCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var list = GameManager.Instance.World.Players.list;
            foreach (var player1 in list)
            {
                if (player1.IsDead())
                {
                    foreach (var player2 in list)
                    {
                        if (player1 == player2)
                        {
                            continue;
                        }

                        var target = player2.GetDamagedTarget();
                        if (target == player1)
                        {
                            var victim = ConnectionManager.Instance.Clients.ForEntityId(player1.entityId);
                            var attacker = ConnectionManager.Instance.Clients.ForEntityId(player2.entityId);
                            var weapon = player2.inventory.holdingItem.Name;

                            this.WriteLine("{0:L} killed {1:L} with {2:s}", attacker, victim, weapon);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes a log line.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="args">The arguments for the message.</param>
        private void WriteLine(string message, params object[] args)
        {
            var time = DateTime.Now.ToString("MM/dd/yyyy - HH:mm:ss");
            var line = this.GetString(message, SMClient.Console, args);
            this.log.WriteLine($"L {time}: {line}");
            this.log.Flush();
        }
    }
}
