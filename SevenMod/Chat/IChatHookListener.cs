// <copyright file="IChatHookListener.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    /// <summary>
    /// Represents a custom chat hook listener.
    /// </summary>
    public interface IChatHookListener
    {
        /// <summary>
        /// Called when a client sends a chat message.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="message">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to consume the message.</returns>
        bool OnChatMessage(ClientInfo client, string message);
    }
}
