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
        /// Gets the <see cref="PluginContainer"/> object containing this plugin.
        /// </summary>
        PluginContainer Container { get; }

        /// <summary>
        /// Called when the configuration files have been loaded.
        /// </summary>
        void OnConfigsExecuted();

        /// <summary>
        /// Called when the plugin is loaded. Perform any initial setup tasks here.
        /// </summary>
        void OnLoadPlugin();

        /// <summary>
        /// Called when the plugin is unloaded. Perform any cleanup tasks here.
        /// </summary>
        void OnUnloadPlugin();

        /// <summary>
        /// Called when the admins list has been cleared.
        /// </summary>
        void OnReloadAdmins();

        /// <summary>
        /// Called when a chunk has its colors calculated.
        /// </summary>
        /// <param name="chunk">The <see cref="Chunk"/> object representing the chunk.</param>
        void OnCalcChunkColorsDone(Chunk chunk);

        /// <summary>
        /// Called when the server is ready for interaction.
        /// </summary>
        void OnGameAwake();

        /// <summary>
        /// Called when the server is about to shut down.
        /// </summary>
        void OnGameShutdown();

        /// <summary>
        /// Called once the server is ready for players to join.
        /// </summary>
        void OnGameStartDone();

        /// <summary>
        /// Called when a player disconnects from the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="shutdown">A value indicating whether the server is shutting down.</param>
        void OnPlayerDisconnected(ClientInfo client, bool shutdown);

        /// <summary>
        /// Called when a player first connects to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player. May be <c>null</c>.</param>
        /// <param name="rejectReason">A <see cref="StringBuilder"/> object to contain the reason for rejecting the client.</param>
        /// <returns><c>true</c> to accept the client; <c>false</c> to reject the client.</returns>
        bool OnPlayerLogin(ClientInfo client, StringBuilder rejectReason);

        /// <summary>
        /// Called every time a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="respawnReason">The <see cref="RespawnType"/> value indicating the reason for the player spawning.</param>
        /// <param name="pos">The <see cref="Vector3i"/> object representing the position of the player in the world.</param>
        void OnPlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos);

        /// <summary>
        /// Called immediately before a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="chunkViewDim">TODO: Find out what this is.</param>
        /// <param name="playerProfile">The <see cref="PlayerProfile"/> object representing the player's persistent profile.</param>
        void OnPlayerSpawning(ClientInfo client, int chunkViewDim, PlayerProfile playerProfile);

        /// <summary>
        /// Called when a player data file is saved to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="playerDataFile">The <see cref="PlayerDataFile"/> object representing the player's data file.</param>
        void OnSavePlayerData(ClientInfo client, PlayerDataFile playerDataFile);
    }
}
