// <copyright file="SMChatType.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    /// <summary>
    /// Enumeration of chat message types.
    /// </summary>
    public enum SMChatType
    {
        /// <summary>
        /// A global chat message.
        /// </summary>
        Global = 0,

        /// <summary>
        /// A friends-only chat message.
        /// </summary>
        Friends = 1,

        /// <summary>
        /// A party-only chat message.
        /// </summary>
        Party = 2,

        /// <summary>
        /// A whisper.
        /// </summary>
        Whisper = 3
    }
}
