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
    using SevenMod.Lang;

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
        public virtual void OnPlayerDisconnected(SMClient client, bool shutdown)
        {
        }

        /// <inheritdoc/>
        public virtual bool OnPlayerLogin(SMClient client, StringBuilder rejectReason)
        {
            return true;
        }

        /// <inheritdoc/>
        public virtual void OnPlayerSpawnedInWorld(SMClient client, SMRespawnType respawnReason, Pos pos)
        {
        }

        /// <inheritdoc/>
        public virtual void OnPlayerSpawning(SMClient client)
        {
        }

        /// <inheritdoc/>
        public virtual void OnReloadAdmins()
        {
        }

        /// <inheritdoc/>
        public virtual void OnSavePlayerData(SMClient client)
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
            return ConVarManager.FindConVar(this, name);
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
        /// Loads a set of translations.
        /// </summary>
        /// <param name="name">The name of the set.</param>
        protected void LoadTranslations(string name)
        {
            Language.LoadPhrases(this, name);
        }

        /// <summary>
        /// Checks if a translation phrase exists.
        /// </summary>
        /// <param name="phrase">The translation phrase.</param>
        /// <returns><c>true</c> if the translation phrase exists; otherwise <c>false</c>.</returns>
        protected bool TranslationPhraseExists(string phrase)
        {
            return Language.PhraseExists(phrase);
        }

        /// <summary>
        /// Gets a string translated into a client's language.
        /// </summary>
        /// <param name="phrase">The translation phrase.</param>
        /// <param name="client">The <see cref="SMClient"/> object representing the client.</param>
        /// <param name="args">The values for the phrase arguments.</param>
        /// <returns>The translated phrase.</returns>
        protected string GetString(string phrase, SMClient client, params object[] args)
        {
            if (!Language.PhraseExists(phrase))
            {
                return phrase;
            }

            return Language.GetString(phrase, client?.ClientInfo, args);
        }

        /// <summary>
        /// Gets a string translated into a server's default language.
        /// </summary>
        /// <param name="phrase">The translation phrase.</param>
        /// <param name="args">The values for the phrase arguments.</param>
        /// <returns>The translated phrase.</returns>
        protected string GetString(string phrase, params object[] args)
        {
            return Language.GetString(phrase, null, args);
        }

        /// <summary>
        /// Find an existing admin command with the specified name.
        /// </summary>
        /// <param name="command">The name of the admin command to locate.</param>
        /// <returns>The <see cref="AdminCommand"/> object representing the admin command if found; otherwise <c>null</c>.</returns>
        protected AdminCommand FindAdminCommand(string command)
        {
            return AdminCommandManager.FindCommand(this, command);
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
            return AdminCommandManager.CreateCommand(this, cmd, accessFlags, description);
        }

        /// <summary>
        /// Removes an <see cref="AdminCommand"/>.
        /// </summary>
        /// <param name="cmd">The name of the admin command.</param>
        protected void UnregAdminCommand(string cmd)
        {
            AdminCommandManager.RemoveCommand(this, cmd);
        }

        /// <summary>
        /// Executes a command on the server's console.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <param name="client">The <see cref="SMClient"/> object representing the client executing the command.</param>
        protected void ServerCommand(string cmd, SMClient client = null)
        {
            SdtdConsole.Instance.ExecuteSync(cmd, client?.ClientInfo);
        }

        /// <summary>
        /// Sends a chat message to an individual client.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments for the message.</param>
        protected void PrintToChat(SMClient client, string message, params object[] args)
        {
            ChatHelper.SendTo(client.ClientInfo, null, message, args);
        }

        /// <summary>
        /// Sends a chat message to an individual client with a name attached.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client.</param>
        /// <param name="sender">The name to display as the sender of the message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments for the message.</param>
        protected void PrintToChatFrom(SMClient client, string sender, string message, params object[] args)
        {
            ChatHelper.SendTo(client.ClientInfo, sender, message, args);
        }

        /// <summary>
        /// Sends a chat message to all connected clients.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments for the message.</param>
        protected void PrintToChatAll(string message, params object[] args)
        {
            ChatHelper.SendToAll(null, message, args);
        }

        /// <summary>
        /// Sends a chat message to all connected clients with a name attached.
        /// </summary>
        /// <param name="sender">The name to display as the sender of the message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="args">The arguments for the message.</param>
        protected void PrintToChatAllFrom(string sender, string message, params object[] args)
        {
            ChatHelper.SendToAll(sender, message, args);
        }

        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the client used to call the currently executing command.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing calling client information.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="args">The arguments for the message.</param>
        protected void ReplyToCommand(SMClient client, string message, params object[] args)
        {
            ChatHelper.ReplyToCommand(client?.ClientInfo, message, args);
        }

        /// <summary>
        /// Checks whether commands should reply to a user via chat.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client for which to reply.</param>
        /// <returns><c>true</c> to reply via chat; <c>false</c> to reply via console.</returns>
        protected bool ShouldReplyToChat(SMClient client)
        {
            return ChatHook.ShouldReplyToChat(client?.ClientInfo);
        }

        /// <summary>
        /// Parse a player target string into a list of currently connected clients.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the source client.</param>
        /// <param name="targetString">The player target string.</param>
        /// <returns>A list of <see cref="SMClient"/> objects representing the matching clients.</returns>
        protected List<SMClient> ParseTargetString(SMClient client, string targetString)
        {
            return SMConsoleHelper.ParseTargetString(client?.ClientInfo, targetString);
        }

        /// <summary>
        /// Parse a single player target string into a connected client.
        /// </summary>
        /// <param name="client">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <param name="target">Variable to be set to the <see cref="SMClient"/> object representing the matching client.</param>
        /// <returns><c>true</c> if a match is found; otherwise <c>false</c>.</returns>
        protected bool ParseSingleTargetString(SMClient client, string targetString, out SMClient target)
        {
            return SMConsoleHelper.ParseSingleTargetString(client?.ClientInfo, targetString, out target);
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
