// <copyright file="SMRespawnType.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Enumeration of player spawn reasons.
    /// </summary>
    public enum SMRespawnType : byte
    {
        /// <summary>
        /// A new game was started.
        /// </summary>
        NewGame = 0,

        /// <summary>
        /// The game was loaded.
        /// </summary>
        LoadedGame = 1,

        /// <summary>
        /// The player died.
        /// </summary>
        Died = 2,

        /// <summary>
        /// The player was teleported.
        /// </summary>
        Teleport = 3,

        /// <summary>
        /// The player entered a multiplayer game.
        /// </summary>
        EnterMultiplayer = 4,

        /// <summary>
        /// The player joined a multiplayer game.
        /// </summary>
        JoinMultiplayer = 5,

        /// <summary>
        /// Unknown reason.
        /// </summary>
        Unknown = 6
    }
}
