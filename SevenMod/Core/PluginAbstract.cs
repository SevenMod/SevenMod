// <copyright file="PluginAbstract.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Text;
    using SevenMod.Admin;
    using SevenMod.Console;

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
        public virtual void ConfigsExecuted()
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
        /// Find an existing console variable with the specified name.
        /// </summary>
        /// <param name="name">The name of the console variable to locate.</param>
        /// <returns>The <see cref="ConVar"/> representing the console variable if found; otherwise
        /// <c>null</c>.</returns>
        protected ConVar FindConVar(string name)
        {
            return ConVarManager.FindConVar(name);
        }

        /// <summary>
        /// Creates a new <see cref="ConVar"/> or returns the existing one if one with the same
        /// name already exists.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="defaultValue">The default value of the variable as a string.</param>
        /// <param name="description">An optional description for the variable.</param>
        /// <param name="hasMin">Optional value indicating whether the variable has a minimum
        /// value.</param>
        /// <param name="min">The minimum value of the variable if <paramref name="hasMin"/> is
        /// <c>true</c>.</param>
        /// <param name="hasMax">Optional value indicating whether the variable has a maximum
        /// value.</param>
        /// <param name="max">The maximum value of the variable if <paramref name="hasMax"/> is
        /// <c>true</c>.</param>
        /// <returns>An instance of the <see cref="ConVar"/> representing the console
        /// variable.</returns>
        protected ConVar CreateConVar(string name, string defaultValue, string description = "", bool hasMin = false, float min = 0.0f, bool hasMax = false, float max = 1.0f)
        {
            return ConVarManager.CreateConVar(this, name, defaultValue, description, hasMin, min, hasMax, max);
        }

        /// <summary>
        /// Adds a configuration file to be automatically loaded.
        /// </summary>
        /// <param name="autoCreate">A value indicating whether the file should be automatically
        /// created if it does not exist.</param>
        /// <param name="name">The name of the configuration file without extension.</param>
        protected void AutoExecConfig(bool autoCreate, string name)
        {
            ConVarManager.AutoExecConfig(this, autoCreate, name);
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
