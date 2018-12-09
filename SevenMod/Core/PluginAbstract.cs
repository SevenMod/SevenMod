// <copyright file="PluginAbstract.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;
    using System.Text;
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Console;
    using SevenMod.ConVar;

    /// <summary>
    /// Represents the default implementation of the <see cref="IPlugin"/> interface.
    /// </summary>
    public abstract class PluginAbstract : IPlugin
    {
        /// <inheritdoc/>
        public abstract PluginInfo Info { get; }

        /// <inheritdoc/>
        public PluginContainer Container { get; internal set; }

        /// <inheritdoc/>
        public virtual void OnCalcChunkColorsDone(Chunk chunk)
        {
        }

        /// <inheritdoc/>
        public virtual void OnConfigsExecuted()
        {
        }

        /// <inheritdoc/>
        public virtual void OnGameAwake()
        {
        }

        /// <inheritdoc/>
        public virtual void OnGameShutdown()
        {
        }

        /// <inheritdoc/>
        public virtual void OnGameStartDone()
        {
        }

        /// <inheritdoc/>
        public virtual void OnLoadPlugin()
        {
        }

        /// <inheritdoc/>
        public virtual void OnPlayerDisconnected(ClientInfo client, bool shutdown)
        {
        }

        /// <inheritdoc/>
        public virtual bool OnPlayerLogin(ClientInfo client, StringBuilder rejectReason)
        {
            return true;
        }

        /// <inheritdoc/>
        public virtual void OnPlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos)
        {
        }

        /// <inheritdoc/>
        public virtual void OnPlayerSpawning(ClientInfo client, int chunkViewDim, PlayerProfile playerProfile)
        {
        }

        /// <inheritdoc/>
        public virtual void OnReloadAdmins()
        {
        }

        /// <inheritdoc/>
        public virtual void OnSavePlayerData(ClientInfo client, PlayerDataFile playerDataFile)
        {
        }

        /// <inheritdoc/>
        public virtual void OnUnloadPlugin()
        {
        }

        /// <summary>
        /// Find an existing console variable with the specified name.
        /// </summary>
        /// <param name="name">The name of the console variable to locate.</param>
        /// <returns>The <see cref="ConVar"/> object representing the console variable if found; otherwise <c>null</c>.</returns>
        protected ConVar FindConVar(string name)
        {
            return ConVarManager.FindConVar(name);
        }

        /// <summary>
        /// Creates a new <see cref="ConVar"/> or returns the existing one if one with the same name already exists.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="defaultValue">The default value of the variable as a string.</param>
        /// <param name="description">Optional description for the variable.</param>
        /// <param name="hasMin">Optional value indicating whether the variable has a minimum value.</param>
        /// <param name="min">The minimum value of the variable if <paramref name="hasMin"/> is <c>true</c>.</param>
        /// <param name="hasMax">Optional value indicating whether the variable has a maximum value.</param>
        /// <param name="max">The maximum value of the variable if <paramref name="hasMax"/> is <c>true</c>.</param>
        /// <returns>The <see cref="ConVar"/> object representing the console variable.</returns>
        protected ConVar CreateConVar(string name, string defaultValue, string description = "", bool hasMin = false, float min = 0.0f, bool hasMax = false, float max = 1.0f)
        {
            return ConVarManager.CreateConVar(this, name, defaultValue, description, hasMin, min, hasMax, max);
        }

        /// <summary>
        /// Adds a configuration file to be automatically loaded.
        /// </summary>
        /// <param name="autoCreate">A value indicating whether the file should be automatically created if it does not exist.</param>
        /// <param name="name">The name of the configuration file without extension.</param>
        protected void AutoExecConfig(bool autoCreate, string name)
        {
            ConVarManager.AutoExecConfig(this, autoCreate, name);
        }

        /// <summary>
        /// Creates a new <see cref="AdminCommand"/> or returns the existing one if one with the same name already exists.
        /// </summary>
        /// <param name="cmd">The name of the admin command.</param>
        /// <param name="accessFlags">The <see cref="AdminFlags"/> value required to execute the admin command.</param>
        /// <param name="description">An optional description for the admin command.</param>
        /// <returns>The <see cref="AdminCommand"/> object representing the admin command.</returns>
        protected AdminCommand RegAdminCmd(string cmd, AdminFlags accessFlags, string description = "")
        {
            return AdminCommandManager.CreateAdminCommand(this, cmd, accessFlags, description);
        }

        /// <summary>
        /// Removes an <see cref="AdminCommand"/>.
        /// </summary>
        /// <param name="cmd">The name of the admin command.</param>
        protected void UnregAdminCommand(string cmd)
        {
            AdminCommandManager.RemoveAdminCommand(this, cmd);
        }

        /// <summary>
        /// Executes a command on the server's console.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        protected void ServerCommand(string cmd)
        {
            SdtdConsole.Instance.ExecuteSync(cmd, null);
        }

        /// <summary>
        /// Sends a chat message to an individual client.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client.</param>
        /// <param name="message">The message text.</param>
        /// <param name="prefix">Optional prefix for the message.</param>
        /// <param name="name">Optional name to attach to the message.</param>
        protected void PrintToChat(ClientInfo client, string message, string prefix = "SM", string name = null)
        {
            ChatHelper.SendTo(client, message, prefix, name);
        }

        /// <summary>
        /// Sends a chat message to all connected clients.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="prefix">Optional prefix for the message.</param>
        /// <param name="name">Optional name to attach to the message.</param>
        protected void PrintToChatAll(string message, string prefix = "SM", string name = null)
        {
            ChatHelper.SendToAll(message, prefix, name);
        }

        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the client used to call the currently executing command.
        /// </summary>
        /// <param name="senderInfo">The <see cref="CommandSenderInfo"/> object representing calling client information.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="prefix">Optional prefix for the message.</param>
        protected void ReplyToCommand(CommandSenderInfo senderInfo, string message, string prefix = "SM")
        {
            ChatHelper.ReplyToCommand(senderInfo, message, prefix);
        }

        /// <summary>
        /// Checks whether commands should reply to a user via chat.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client for which to reply.</param>
        /// <returns><c>true</c> to reply via chat; <c>false</c> to reply via console.</returns>
        protected bool ShouldReplyToChat(ClientInfo client)
        {
            return ChatHook.ShouldReplyToChat(client);
        }

        /// <summary>
        /// Parse a player target string into a list of currently connected clients.
        /// </summary>
        /// <param name="senderInfo">The <see cref="CommandSenderInfo"/> object representing the source client.</param>
        /// <param name="targetString">The player target string.</param>
        /// <returns>A list of <see cref="ClientInfo"/> objects representing the matching clients.</returns>
        protected List<ClientInfo> ParseTargetString(CommandSenderInfo senderInfo, string targetString)
        {
            return SMConsoleHelper.ParseTargetString(senderInfo, targetString);
        }

        /// <summary>
        /// Parse a single player target string into a connected client.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <param name="client">Variable to be set to the <see cref="ClientInfo"/> object representing the matching client.</param>
        /// <returns><c>true</c> if a match is found; otherwise <c>false</c>.</returns>
        protected bool ParseSingleTargetString(CommandSenderInfo senderInfo, string targetString, out ClientInfo client)
        {
            return SMConsoleHelper.ParseSingleTargetString(senderInfo, targetString, out client);
        }

        /// <summary>
        /// Logs a message to the SevenMod logs.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected void LogMessage(string message)
        {
            SMLog.Out(message, this.Container.File);
        }

        /// <summary>
        /// Logs a message to the SevenMod error logs.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        protected void LogError(string message)
        {
            SMLog.Error(message, this.Container.File);
        }

        /// <summary>
        /// Sets the plugin to an error state.
        /// </summary>
        /// <param name="error">The error message.</param>
        protected void SetFailState(string error)
        {
            this.Container.SetFailState(error);
            throw new HaltPluginException(error);
        }
    }
}
