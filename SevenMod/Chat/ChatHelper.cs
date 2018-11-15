﻿// <copyright file="ChatHelper.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    /// <summary>
    /// Chat related utilities.
    /// </summary>
    public class ChatHelper
    {
        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the
        /// client used to call the currently executing command.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="message">The message to send.</param>
        public static void ReplyToCommand(CommandSenderInfo senderInfo, string message)
        {
            if ((senderInfo.RemoteClientInfo != null) && ChatHook.ShouldReplyToChat(senderInfo.RemoteClientInfo))
            {
                SendTo(senderInfo.RemoteClientInfo, message);
            }
            else
            {
                SdtdConsole.Instance.Output(message);
            }
        }

        /// <summary>
        /// Sends a chat message to an individual client.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client.</param>
        /// <param name="message">The message text.</param>
        /// <param name="prefix">The prefix for the message.</param>
        public static void SendTo(ClientInfo client, string message, string prefix = "[SM]")
        {
            client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, message, prefix, false, "SevenMod", false));
        }

        /// <summary>
        /// Sends a chat message to all connected clients.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="prefix">The prefix for the message.</param>
        public static void SendToAll(string message, string prefix = "[SM]")
        {
            var package = new NetPackageGameMessage(EnumGameMessages.Chat, message, prefix, false, "SevenMod", false);
            foreach (var player in ConnectionManager.Instance.GetClients())
            {
                player.SendPackage(package);
            }
        }
    }
}
