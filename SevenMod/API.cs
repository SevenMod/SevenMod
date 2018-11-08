// <copyright file="API.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// Represents the entry point of the mod.
    /// </summary>
    public class API : ModApiAbstract
    {
        /// <summary>
        /// The active plugins.
        /// </summary>
        private List<PluginAbstract> plugins = new List<PluginAbstract>();

        /// <inheritdoc/>
        public override void CalcChunkColorsDone(Chunk _chunk)
        {
            foreach (var plugin in this.plugins)
            {
                plugin.CalcChunkColorsDone(_chunk);
            }
        }

        /// <inheritdoc/>
        public override bool ChatMessage(ClientInfo _cInfo, EnumGameMessages _type, string _msg, string _mainName, bool _localizeMain, string _secondaryName, bool _localizeSecondary)
        {
            return ChatHook.HookChatMessage(_cInfo, _msg);
        }

        /// <inheritdoc/>
        public override void GameAwake()
        {
            ConfigManager.Init();
            this.plugins = ConfigManager.GetPlugins();
            foreach (var plugin in this.plugins)
            {
                plugin.LoadPlugin();
            }

            foreach (var plugin in this.plugins)
            {
                plugin.GameAwake();
            }
        }

        /// <inheritdoc/>
        public override void GameShutdown()
        {
            foreach (var plugin in this.plugins)
            {
                plugin.GameShutdown();
            }

            foreach (var plugin in this.plugins)
            {
                plugin.UnloadPlugin();
            }
        }

        /// <inheritdoc/>
        public override void GameStartDone()
        {
            foreach (var plugin in this.plugins)
            {
                plugin.GameStartDone();
            }
        }

        /// <inheritdoc/>
        public override void GameUpdate()
        {
            foreach (var plugin in this.plugins)
            {
                plugin.GameUpdate();
            }
        }

        /// <inheritdoc/>
        public override void PlayerDisconnected(ClientInfo _cInfo, bool _bShutdown)
        {
            foreach (var plugin in this.plugins)
            {
                plugin.PlayerDisconnected(_cInfo, _bShutdown);
            }
        }

        /// <inheritdoc/>
        public override void PlayerLogin(ClientInfo _cInfo, string _compatibilityVersion)
        {
            foreach (var plugin in this.plugins)
            {
                plugin.PlayerLogin(_cInfo, _compatibilityVersion);
            }
        }

        /// <inheritdoc/>
        public override void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            foreach (var plugin in this.plugins)
            {
                plugin.PlayerSpawnedInWorld(_cInfo, _respawnReason, _pos);
            }
        }

        /// <inheritdoc/>
        public override void PlayerSpawning(ClientInfo _cInfo, int _chunkViewDim, PlayerProfile _playerProfile)
        {
            foreach (var plugin in this.plugins)
            {
                plugin.PlayerSpawning(_cInfo, _chunkViewDim, _playerProfile);
            }
        }

        /// <inheritdoc/>
        public override void SavePlayerData(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
        {
            foreach (var plugin in this.plugins)
            {
                plugin.SavePlayerData(_cInfo, _playerDataFile);
            }
        }
    }
}
