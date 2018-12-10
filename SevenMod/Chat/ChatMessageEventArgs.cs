// <copyright file="ChatMessageEventArgs.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    using System;
    using SevenMod.Core;

    /// <summary>
    /// Contains the arguments for the <see cref="ChatHook.ChatMessage"/> event.
    /// </summary>
    public class ChatMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMessageEventArgs"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="message">The message text.</param>
        internal ChatMessageEventArgs(ClientInfo client, string message)
        {
            this.Client = new SMClient(client);
            this.Message = message;
        }

        /// <summary>
        /// Gets the <see cref="ClientInfo"/> object representing the client that sent the message.
        /// </summary>
        public SMClient Client { get; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the message has been handled.
        /// </summary>
        public bool Handled { get; set; }
    }
}
