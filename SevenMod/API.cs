// <copyright file="API.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        /// Gets a value indicating whether <see cref="OnGameAwake"/> has been called.
        /// </summary>
        public static bool IsGameAwake { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="OnGameStartDone"/> has been called.
        /// </summary>
        public static bool IsGameStartDone { get; private set; }

        /// <inheritdoc/>
        public void InitMod()
        {
            ModEvents.ChatMessage.RegisterHandler((ClientInfo cInfo, EChatType chatType, int senderEntityId, string msg, string mainName, bool localizeMain, List<int> recipientEntityIds) => this.OnChatMessage(cInfo, chatType, msg));
            ModEvents.GameAwake.RegisterHandler(this.OnGameAwake);
            ModEvents.GameShutdown.RegisterHandler(this.OnGameShutdown);
            ModEvents.GameStartDone.RegisterHandler(this.OnGameStartDone);
            ModEvents.PlayerDisconnected.RegisterHandler(this.OnPlayerDisconnected);
            ModEvents.PlayerLogin.RegisterHandler((ClientInfo cInfo, string compatibilityVersion, StringBuilder rejectReason) => this.OnPlayerLogin(cInfo, rejectReason));
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(this.OnPlayerSpawnedInWorld);
            ModEvents.PlayerSpawning.RegisterHandler((ClientInfo cInfo, int chunkViewDim, PlayerProfile playerProfile) => this.OnPlayerSpawning(cInfo));
            ModEvents.SavePlayerData.RegisterHandler((ClientInfo cInfo, PlayerDataFile playerDataFile) => this.OnSavePlayerData(cInfo));

            ConVarManager.AutoExecConfig(null, true, "Core");
            ChatHook.Init();
            PluginManager.Refresh();
        }

        /// <summary>
        /// Called when a chat message is sent.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="type">The type of chat message.</param>
        /// <param name="msg">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to consume the message.</returns>
        private bool OnChatMessage(ClientInfo client, EChatType type, string msg)
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
        private void OnGameAwake()
        {
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnGameAwake();
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }

            IsGameAwake = true;
        }

        /// <summary>
        /// Called when the server is about to shut down.
        /// </summary>
        private void OnGameShutdown()
        {
            PluginManager.IsLocked = true;
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnGameShutdown();
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }

            PluginManager.UnloadAll();
            SMLog.Close();
        }

        /// <summary>
        /// Called once the server is ready for players to join.
        /// </summary>
        private void OnGameStartDone()
        {
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnGameStartDone();
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }

            IsGameStartDone = true;
        }

        /// <summary>
        /// Called when a player disconnects from the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        /// <param name="shutdown">A value indicating whether the server is shutting down.</param>
        private void OnPlayerDisconnected(ClientInfo client, bool shutdown)
        {
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnPlayerDisconnected(new SMClient(client), shutdown);
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }
        }

        /// <summary>
        /// Called when a player first connects to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player. May be <c>null</c>.</param>
        /// <param name="rejectReason">A <see cref="StringBuilder"/> object to contain the reason for rejecting the client.</param>
        /// <returns><c>true</c> to accept the client; <c>false</c> to reject the client.</returns>
        private bool OnPlayerLogin(ClientInfo client, StringBuilder rejectReason)
        {
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        if (!plugin.Plugin.OnPlayerLogin(new SMClient(client), rejectReason))
                        {
                            return false;
                        }
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
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
        private void OnPlayerSpawnedInWorld(ClientInfo client, RespawnType respawnReason, Vector3i pos)
        {
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnPlayerSpawnedInWorld(new SMClient(client), (SMRespawnType)respawnReason, new Pos(pos));
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }
        }

        /// <summary>
        /// Called immediately before a player spawns into the world.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        private void OnPlayerSpawning(ClientInfo client)
        {
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnPlayerSpawning(new SMClient(client));
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }
        }

        /// <summary>
        /// Called when a player data file is saved to the server.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the player.</param>
        private void OnSavePlayerData(ClientInfo client)
        {
            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnSavePlayerData(new SMClient(client));
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }
        }
    }
}
