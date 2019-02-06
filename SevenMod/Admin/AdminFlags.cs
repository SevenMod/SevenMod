// <copyright file="AdminFlags.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Admin
{
    /// <summary>
    /// Enumeration of access flags used to grant users access to admin commands and features.
    /// </summary>
    public enum AdminFlags : int
    {
        /// <summary>
        /// Flag <c>a</c>: Reserved slots
        /// </summary>
        Reservation = 1 << 0,

        /// <summary>
        /// Flag <c>b</c>: Generic admin
        /// </summary>
        Generic = 1 << 1,

        /// <summary>
        /// Flag <c>c</c>: Kick other players
        /// </summary>
        Kick = 1 << 2,

        /// <summary>
        /// Flag <c>d</c>: Ban other players
        /// </summary>
        Ban = 1 << 3,

        /// <summary>
        /// Flag <c>e</c>: Remove bans
        /// </summary>
        Unban = 1 << 4,

        /// <summary>
        /// Flag <c>f</c>: Damage or kill other players
        /// </summary>
        Slay = 1 << 5,

        /// <summary>
        /// Flag <c>g</c>: Map/world related functions
        /// </summary>
        Changemap = 1 << 6,

        /// <summary>
        /// Flag <c>h</c>: Change server settings
        /// </summary>
        Convars = 1 << 7,

        /// <summary>
        /// Flag <c>i</c>: Execute configuration files
        /// </summary>
        Config = 1 << 8,

        /// <summary>
        /// Flag <c>j</c>: Special chat privileges
        /// </summary>
        Chat = 1 << 9,

        /// <summary>
        /// Flag <c>k</c>: Start votes
        /// </summary>
        Vote = 1 << 10,

        /// <summary>
        /// Flag <c>l</c>: Password the server
        /// </summary>
        Password = 1 << 11,

        /// <summary>
        /// Flag <c>m</c>: Server console access
        /// </summary>
        RCON = 1 << 12,

        /// <summary>
        /// Flag <c>n</c>: "Cheat" commands
        /// </summary>
        Cheats = 1 << 13,

        /// <summary>
        /// Flag <c>z</c>: Root access
        /// </summary>
        Root = 1 << 14,

        /// <summary>
        /// Flag <c>o</c>
        /// </summary>
        Custom1 = 1 << 15,

        /// <summary>
        /// Flag <c>p</c>
        /// </summary>
        Custom2 = 1 << 16,

        /// <summary>
        /// Flag <c>q</c>
        /// </summary>
        Custom3 = 1 << 17,

        /// <summary>
        /// Flag <c>r</c>
        /// </summary>
        Custom4 = 1 << 18,

        /// <summary>
        /// Flag <c>s</c>
        /// </summary>
        Custom5 = 1 << 19,

        /// <summary>
        /// Flag <c>t</c>
        /// </summary>
        Custom6 = 1 << 20,
    }
}
