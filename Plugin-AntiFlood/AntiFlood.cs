// <copyright file="AntiFlood.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.AntiFlood
{
    using System;
    using System.Collections.Generic;
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that protects against chat flooding.
    /// </summary>
    public class AntiFlood : PluginAbstract
    {
        /// <summary>
        /// The value of the FloodTime <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue floodTime;

        /// <summary>
        /// The time until each client is allowed to send a chat message.
        /// </summary>
        private Dictionary<string, long> minChatTimes = new Dictionary<string, long>();

        /// <summary>
        /// The number of tokens each client has accumulated.
        /// </summary>
        private Dictionary<string, int> tokens = new Dictionary<string, int>();

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Anti-Flood",
            Author = "SevenMod",
            Description = "Protects against chat flooding.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.floodTime = this.CreateConVar("FloodTime", "0.75", "The time in seconds allowed between chat messages.", true, 0).Value;

            this.AutoExecConfig(true, "AntiFlood");

            ChatHook.ChatMessage += this.OnChatMessage;
        }

        /// <inheritdoc/>
        public override void OnPlayerDisconnected(SMClient client, bool shutdown)
        {
            this.minChatTimes.Remove(client.PlayerId);
            this.tokens.Remove(client.PlayerId);
        }

        /// <summary>
        /// Called when a chat message is received from a client.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ChatMessageEventArgs"/> object containing the event data.</param>
        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            if (AdminManager.CheckAccess(e.Client, AdminFlags.Root))
            {
                return;
            }

            this.minChatTimes.TryGetValue(e.Client.PlayerId, out var minChatTime);
            this.tokens.TryGetValue(e.Client.PlayerId, out var token);

            var time = DateTime.Now.Ticks;
            var nextTime = time + (long)(this.floodTime.AsFloat * 10000000);

            if (time < minChatTime)
            {
                if (token < 3)
                {
                    this.tokens[e.Client.PlayerId] = token + 1;
                }
                else
                {
                    nextTime += 30000000;
                    e.Handled = true;
                }
            }
            else if (token > 0)
            {
                this.tokens[e.Client.PlayerId] = token - 1;
            }

            this.minChatTimes[e.Client.PlayerId] = nextTime;
        }
    }
}
