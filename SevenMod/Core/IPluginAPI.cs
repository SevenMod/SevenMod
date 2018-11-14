// <copyright file="IPluginAPI.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Represents a plugin.
    /// </summary>
    public interface IPluginAPI
    {
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
        /// <param name="chunk">An instance of the <see cref="Chunk"/> class representing the
        /// chunk.</param>
        void CalcChunkColorsDone(Chunk chunk);

        /// <summary>
        /// Called when the server is ready for interaction. At this point,
        /// <c>GameManager.Instance.World</c> is set.
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
        /// Called every time the game executes an update. This happens very frequently, so don't
        /// perform any intensive tasks here without rate limiting.
        /// </summary>
        void GameUpdate();

        /// <summary>
        /// Called when a player disconnects from the server.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/></param> class
        /// representing the player.
        /// <param name="shutdown">TODO: Find out what this is.</param>
        void PlayerDisconnected(ClientInfo client, bool shutdown);

        /// <summary>
        /// Called when a player first connects to the server.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/></param> class
        /// representing the player. May be <c>null</c>.
        /// <param name="compatibilityVersion">TODO: Find out what this is.</param>
        void PlayerLogin(ClientInfo client, string compatibilityVersion);

        /// <summary>
        /// Called every time a player spawns into the world.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/></param> class
        /// representing the player.
        /// <param name="respawnReason">A <see cref="RespawnType"/> value indicating the reason for
        /// the player spawning.</param>
        /// <param name="pos">An instance of the <see cref="Vector3i"/> class representing the
        /// position of the player in the world.</param>
        void PlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos);

        /// <summary>
        /// Called immediately before a player spawns into the world.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/></param> class
        /// representing the player.
        /// <param name="chunkViewDim">TODO: Find out what this is.</param>
        /// <param name="playerProfile">An instance of the <see cref="PlayerProfile"/> class
        /// representing the player's persistent profile.</param>
        void PlayerSpawning(ClientInfo client, int chunkViewDim, PlayerProfile playerProfile);

        /// <summary>
        /// Called when a player data file is saved to the server.
        /// </summary>
        /// <param name="client">An instance of the <see cref="ClientInfo"/></param> class
        /// representing the player.
        /// <param name="playerDataFile">An instance of the <see cref="PlayerDataFile"/> class
        /// representing the player's data file.</param>
        void SavePlayerData(ClientInfo client, PlayerDataFile playerDataFile);
    }
}
