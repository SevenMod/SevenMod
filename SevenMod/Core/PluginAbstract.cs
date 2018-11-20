// <copyright file="PluginAbstract.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Text;
    using SevenMod.Admin;

    /// <summary>
    /// Represents the default implementation of the <see cref="IPluginAPI"/> interface.
    /// </summary>
    public abstract class PluginAbstract : IPluginAPI
    {
        /// <summary>
        /// Gets the metadata for the plugin.
        /// </summary>
        public abstract PluginInfo Info { get; }

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
        public virtual void LoadPlugin()
        {
        }

        /// <inheritdoc/>
        public virtual void PlayerDisconnected(ClientInfo client, bool shutdown)
        {
        }

        /// <inheritdoc/>
        public virtual bool PlayerLogin(ClientInfo client, StringBuilder rejectReason)
        {
            return true;
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
        public virtual void ReloadAdmins()
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
        /// <param name="command">The name of the command.</param>
        /// <param name="handler">An instance of <see cref="AdminCmdAbstract"/> which will handle
        /// calls to the command.</param>
        /// <param name="accessFlags">The <see cref="AdminFlags"/> required to execute the
        /// command.</param>
        protected void RegAdminCmd(string command, AdminCmdAbstract handler, AdminFlags accessFlags)
        {
            AdminCmdRouter.RegisterAdminCmd(this, command, handler, accessFlags);
        }

        /// <summary>
        /// Structure for containing metadata for a plugin.
        /// </summary>
        public struct PluginInfo
        {
            /// <summary>
            /// The name of the plugin.
            /// </summary>
            public string Name;

            /// <summary>
            /// The name of the plugin author(s).
            /// </summary>
            public string Author;

            /// <summary>
            /// A brief description for the plugin.
            /// </summary>
            public string Description;

            /// <summary>
            /// The version identifier.
            /// </summary>
            public string Version;

            /// <summary>
            /// The website associated with the plugin.
            /// </summary>
            public string Website;
        }
    }
}
