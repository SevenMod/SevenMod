// <copyright file="API.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod
{
    using System.Collections.Generic;
    using System.Text;
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// Represents the entry point of the mod.
    /// </summary>
    public class API : IModApi
    {
        /// <inheritdoc/>
        public void InitMod()
        {
            ModEvents.CalcChunkColorsDone.RegisterHandler(this.CalcChunkColorsDone);
            ModEvents.ChatMessage.RegisterHandler((ClientInfo cInfo, EChatType chatType, int senderEntityId, string msg, string mainName, bool localizeMain, List<int> recipientEntityIds) => this.ChatMessage(cInfo, chatType, msg));
            ModEvents.GameAwake.RegisterHandler(this.GameAwake);
            ModEvents.GameShutdown.RegisterHandler(this.GameShutdown);
            ModEvents.GameStartDone.RegisterHandler(this.GameStartDone);
            ModEvents.PlayerDisconnected.RegisterHandler(this.PlayerDisconnected);
            ModEvents.PlayerLogin.RegisterHandler((ClientInfo cInfo, string compatibilityVersion, StringBuilder rejectReason) => this.PlayerLogin(cInfo, rejectReason));
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(this.PlayerSpawnedInWorld);
            ModEvents.PlayerSpawning.RegisterHandler(this.PlayerSpawning);
            ModEvents.SavePlayerData.RegisterHandler(this.SavePlayerData);
        }

        /// <summary>
        /// Called when a chunk has its colors calculated.
        /// </summary>
        /// <param name="chunk">An instance of the <see cref="Chunk"/> class representing the
        /// chunk.</param>
        private void CalcChunkColorsDone(Chunk chunk)
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.CalcChunkColorsDone(chunk);
            }
        }

        /// <summary>
        /// Called when a chat message is sent.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/> class representing the
        /// client that sent the message.</param>
        /// <param name="type">The type of chat message.</param>
        /// <param name="msg">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to
        /// consume the message.</returns>
        private bool ChatMessage(ClientInfo client, EChatType type, string msg)
        {
            if (type == EChatType.Global)
            {
                return ChatHook.HookChatMessage(client, msg);
            }

            return true;
        }

        /// <summary>
        /// Called when the server is ready for interaction. At this point,
        /// <c>GameManager.Instance.World</c> is set.
        /// </summary>
        private void GameAwake()
        {
            ConfigManager.Init();
            PluginManager.Refresh();
        }

        /// <summary>
        /// Called when the server is about to shut down.
        /// </summary>
        private void GameShutdown()
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.GameShutdown();
            }

            PluginManager.UnloadAll();
        }

        /// <summary>
        /// Called once the server is ready for players to join.
        /// </summary>
        private void GameStartDone()
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.GameStartDone();
            }
        }

        /// <summary>
        /// Called when a player disconnects from the server.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/> class representing the
        /// player.</param>
        /// <param name="shutdown">Value indicating whether the server is shutting down.</param>
        private void PlayerDisconnected(ClientInfo client, bool shutdown)
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.PlayerDisconnected(client, shutdown);
            }
        }

        /// <summary>
        /// Called when a player first connects to the server.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/> class representing the
        /// player. May be <c>null</c>.</param>
        /// <param name="rejectReason"><see cref="StringBuilder"/> object to contain the reason for
        /// rejecting the client.</param>
        /// <returns><c>true</c> to accept the client; <c>false</c> to reject the client.</returns>
        private bool PlayerLogin(ClientInfo client, StringBuilder rejectReason)
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                if (!plugin.PlayerLogin(client, rejectReason))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Called every time a player spawns into the world.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/> class representing the
        /// player.</param>
        /// <param name="respawnReason">A <see cref="RespawnType"/> value indicating the reason for
        /// the player spawning.</param>
        /// <param name="pos">An instance of the <see cref="Vector3i"/> class representing the
        /// position of the player in the world.</param>
        private void PlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos)
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.PlayerSpawnedInWorld(client, respawnReason, pos);
            }
        }

        /// <summary>
        /// Called immediately before a player spawns into the world.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/> class representing the
        /// player.</param>
        /// <param name="chunkViewDim">TODO: Find out what this is.</param>
        /// <param name="playerProfile">An instance of the <see cref="PlayerProfile"/> class
        /// representing the player's persistent profile.</param>
        private void PlayerSpawning(ClientInfo client, int chunkViewDim, PlayerProfile playerProfile)
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.PlayerSpawning(client, chunkViewDim, playerProfile);
            }
        }

        /// <summary>
        /// Called when a player data file is saved to the server.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/> class representing the
        /// player.</param>
        /// <param name="playerDataFile">An instance of the <see cref="PlayerDataFile"/> class
        /// representing the player's data file.</param>
        private void SavePlayerData(ClientInfo client, PlayerDataFile playerDataFile)
        {
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.SavePlayerData(client, playerDataFile);
            }
        }
    }
}
