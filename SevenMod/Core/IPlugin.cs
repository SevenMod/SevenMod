// <copyright file="IPlugin.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Text;

    /// <summary>
    /// Represents a plugin.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the metadata for the plugin.
        /// </summary>
        PluginInfo Info { get; }

        /// <summary>
        /// Called when the configuration files have been loaded.
        /// </summary>
        void ConfigsExecuted();

        /// <summary>
        /// Called when the plugin is loaded. Perform any initial setup tasks here.
        /// </summary>
        void LoadPlugin();

        /// <summary>
        /// Called when the plugin is unloaded. Perform any cleanup tasks here.
        /// </summary>
        void UnloadPlugin();

        /// <summary>
        /// Called when the admins list has been cleared.
        /// </summary>
        void ReloadAdmins();

        /// <summary>
        /// Called when a chunk has its colors calculated.
        /// </summary>
        /// <param name="chunk">The <see cref="Chunk"/> object representing the chunk.</param>
        void CalcChunkColorsDone(Chunk chunk);

        /// <summary>
        /// Called when a chat message is sent.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="msg">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to consume the message.</returns>
        bool ChatMessage(ClientInfo client, string msg);

            /// <summary>
        /// Called when the server is ready for interaction.
        /// </summary>
        void GameAwake();

        /// <summary>
        /// Called when the server is about to shut down.
        /// </summary>
        void GameShutdown();

        /// <summary>
        /// Called once the server is ready for players to join.
        /// </summary>
        void GameStartDone();

        /// <summary>
        /// Called when a player disconnects from the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="shutdown">A value indicating whether the server is shutting down.</param>
        void PlayerDisconnected(ClientInfo client, bool shutdown);

        /// <summary>
        /// Called when a player first connects to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player. May be <c>null</c>.</param>
        /// <param name="rejectReason">A <see cref="StringBuilder"/> object to contain the reason for rejecting the client.</param>
        /// <returns><c>true</c> to accept the client; <c>false</c> to reject the client.</returns>
        bool PlayerLogin(ClientInfo client, StringBuilder rejectReason);

        /// <summary>
        /// Called every time a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="respawnReason">The <see cref="RespawnType"/> value indicating the reason for the player spawning.</param>
        /// <param name="pos">The <see cref="Vector3i"/> object representing the position of the player in the world.</param>
        void PlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos);

        /// <summary>
        /// Called immediately before a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="chunkViewDim">TODO: Find out what this is.</param>
        /// <param name="playerProfile">The <see cref="PlayerProfile"/> object representing the player's persistent profile.</param>
        void PlayerSpawning(ClientInfo client, int chunkViewDim, PlayerProfile playerProfile);

        /// <summary>
        /// Called when a player data file is saved to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="playerDataFile">The <see cref="PlayerDataFile"/> object representing the player's data file.</param>
        void SavePlayerData(ClientInfo client, PlayerDataFile playerDataFile);
    }
}
