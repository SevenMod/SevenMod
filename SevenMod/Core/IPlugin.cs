// <copyright file="IPlugin.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
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
        /// <param name="client">The <see cref="SMClient"/> object representing the player.</param>
        /// <param name="shutdown">A value indicating whether the server is shutting down.</param>
        void OnPlayerDisconnected(SMClient client, bool shutdown);

        /// <summary>
        /// Called when a player first connects to the server.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the player. May be <c>null</c>.</param>
        /// <param name="rejectReason">A <see cref="StringBuilder"/> object to contain the reason for rejecting the client.</param>
        /// <returns><c>true</c> to accept the client; <c>false</c> to reject the client.</returns>
        bool OnPlayerLogin(SMClient client, StringBuilder rejectReason);

        /// <summary>
        /// Called every time a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the player.</param>
        /// <param name="respawnReason">The <see cref="SMRespawnType"/> value indicating the reason for the player spawning.</param>
        /// <param name="pos">The <see cref="Pos"/> object representing the position of the player in the world.</param>
        void OnPlayerSpawnedInWorld(SMClient client, SMRespawnType respawnReason, Pos pos);

        /// <summary>
        /// Called immediately before a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the player.</param>
        void OnPlayerSpawning(SMClient client);

        /// <summary>
        /// Called when a player data file is saved to the server.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the player.</param>
        void OnSavePlayerData(SMClient client);
    }
}
