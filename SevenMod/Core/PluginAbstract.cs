// <copyright file="PluginAbstract.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;

    /// <summary>
    /// Represents the default implementation of the <see cref="IPluginAPI"/> interface.
    /// </summary>
    public abstract class PluginAbstract : IPluginAPI
    {
        /// <inheritdoc/>
        public virtual void CalcChunkColorsDone(Chunk chunk)
        {
        }

        /// <inheritdoc/>
        public virtual void GameAwake()
        {
        }

        /// <inheritdoc/>
        public virtual void GameShutdown()
        {
        }

        /// <inheritdoc/>
        public virtual void GameStartDone()
        {
        }

        /// <inheritdoc/>
        public virtual void GameUpdate()
        {
        }

        /// <inheritdoc/>
        public virtual void LoadPlugin()
        {
        }

        /// <inheritdoc/>
        public virtual void PlayerDisconnected(ClientInfo client, bool shutdown)
        {
        }

        /// <inheritdoc/>
        public virtual void PlayerLogin(ClientInfo client, string compatibilityVersion)
        {
        }

        /// <inheritdoc/>
        public virtual void PlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos)
        {
        }

        /// <inheritdoc/>
        public virtual void PlayerSpawning(ClientInfo client, int chunkViewDim, PlayerProfile playerProfile)
        {
        }

        /// <inheritdoc/>
        public virtual void SavePlayerData(ClientInfo client, PlayerDataFile playerDataFile)
        {
        }

        /// <inheritdoc/>
        public virtual void UnloadPlugin()
        {
        }

        /// <summary>
        /// Registers an admin command.
        /// </summary>
        /// <param name="handler">The <see cref="Type"/> of the <see cref="AdminCmdAbstract"/>
        /// implementation for the command.</param>
        /// <param name="flags">The <see cref="AdminFlags"/> required to execute the command.</param>
        protected static void RegisterAdminCommand(Type handler, AdminFlags flags)
        {
            if (!handler.IsSubclassOf(typeof(AdminCmdAbstract)))
            {
                throw new Exception($"{handler.Name} is not a subclass of AdminCmdAbstract");
            }

            AdminCmdAbstract.Registry.Add(handler, flags);
        }
    }
}
