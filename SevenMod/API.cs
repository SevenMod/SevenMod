// <copyright file="API.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod
{
    using System.Collections.Generic;
    using System.Text;
    using SevenMod.Chat;
    using SevenMod.ConVar;
    using SevenMod.Core;

    /// <summary>
    /// Represents the entry point of the mod.
    /// </summary>
    public class API : IModApi
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="GameAwake"/> has been called.
        /// </summary>
        public static bool IsGameAwake { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="GameStartDone"/> has been called.
        /// </summary>
        public static bool IsGameStartDone { get; private set; }

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

            SMPath.Init();
            ConVarManager.AutoExecConfig(null, true, "Core");
            ChatHook.Init();
            PluginManager.Refresh();
        }

        /// <summary>
        /// Called when a chunk has its colors calculated.
        /// </summary>
        /// <param name="chunk">The <see cref="Chunk"/> object representing the chunk.</param>
        private void CalcChunkColorsDone(Chunk chunk)
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                plugin.CalcChunkColorsDone(chunk);
            }
        }

        /// <summary>
        /// Called when a chat message is sent.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="type">The type of chat message.</param>
        /// <param name="msg">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to consume the message.</returns>
        private bool ChatMessage(ClientInfo client, EChatType type, string msg)
        {
            if (type == EChatType.Global)
            {
                return ChatHook.HookChatMessage(client, msg);
            }

            return true;
        }

        /// <summary>
        /// Called when the server is ready for interaction.
        /// </summary>
        private void GameAwake()
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                plugin.GameAwake();
            }

            IsGameAwake = true;
        }

        /// <summary>
        /// Called when the server is about to shut down.
        /// </summary>
        private void GameShutdown()
        {
            foreach (var plugin in PluginManager.Plugins)
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
            foreach (var plugin in PluginManager.Plugins)
            {
                plugin.GameStartDone();
            }

            IsGameStartDone = true;
        }

        /// <summary>
        /// Called when a player disconnects from the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="shutdown">A value indicating whether the server is shutting down.</param>
        private void PlayerDisconnected(ClientInfo client, bool shutdown)
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                plugin.PlayerDisconnected(client, shutdown);
            }
        }

        /// <summary>
        /// Called when a player first connects to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player. May be <c>null</c>.</param>
        /// <param name="rejectReason">A <see cref="StringBuilder"/> object to contain the reason for rejecting the client.</param>
        /// <returns><c>true</c> to accept the client; <c>false</c> to reject the client.</returns>
        private bool PlayerLogin(ClientInfo client, StringBuilder rejectReason)
        {
            foreach (var plugin in PluginManager.Plugins)
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
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="respawnReason">The <see cref="RespawnType"/> value indicating the reason for the player spawning.</param>
        /// <param name="pos">The <see cref="Vector3i"/> object representing the position of the player in the world.</param>
        private void PlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos)
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                plugin.PlayerSpawnedInWorld(client, respawnReason, pos);
            }
        }

        /// <summary>
        /// Called immediately before a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="chunkViewDim">TODO: Find out what this is.</param>
        /// <param name="playerProfile">The <see cref="PlayerProfile"/> object representing the player's persistent profile.</param>
        private void PlayerSpawning(ClientInfo client, int chunkViewDim, PlayerProfile playerProfile)
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                plugin.PlayerSpawning(client, chunkViewDim, playerProfile);
            }
        }

        /// <summary>
        /// Called when a player data file is saved to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="playerDataFile">The <see cref="PlayerDataFile"/> object representing the player's data file.</param>
        private void SavePlayerData(ClientInfo client, PlayerDataFile playerDataFile)
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                plugin.SavePlayerData(client, playerDataFile);
            }
        }
    }
}
