// <copyright file="MOTD.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.MOTD
{
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: MOTD</para>
    /// <para>Displays a set of messages in chat to players upon connecting to the server.</para>
    /// </summary>
    public class MOTD : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Message of the Day",
            Author = "SevenMod",
            Description = "Displays a set of messages in chat to players upon connecting to the server.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();
            ConfigManager.ParseConfig(MOTDConfig.Instance, "MOTD");
        }

        /// <inheritdoc/>
        public override void PlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i position)
        {
            base.PlayerSpawnedInWorld(client, respawnReason, position);
            if (respawnReason == RespawnType.JoinMultiplayer)
            {
                foreach (var line in MOTDConfig.Instance.Lines)
                {
                    ChatHelper.SendTo(client, line, "MOTD");
                }
            }
        }
    }
}
