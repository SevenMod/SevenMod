// <copyright file="ChatHelper.cs" company="Steve Guidetti">
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
        /// <param name="prefix">The prefix for the message.</param>
        public static void ReplyToCommand(CommandSenderInfo senderInfo, string message, string prefix = "SM")
        {
            if ((senderInfo.RemoteClientInfo != null) && ChatHook.ShouldReplyToChat(senderInfo.RemoteClientInfo))
            {
                SendTo(senderInfo.RemoteClientInfo, message, prefix);
            }
            else
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    message = $"[{prefix}] {message}";
                }

                SdtdConsole.Instance.Output(message);
            }
        }

        /// <summary>
        /// Sends a chat message to an individual client.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client.</param>
        /// <param name="message">The message text.</param>
        /// <param name="prefix">The prefix for the message.</param>
        /// <param name="name">The name to attach to the message.</param>
        public static void SendTo(ClientInfo client, string message, string prefix = "SM", string name = null)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                message = $"[{Colors.Green}][\u200B{prefix}][-] {message}";
            }

            client.SendPackage(new NetPackageChat(EChatType.Global, 0, message, name, false, null));
        }

        /// <summary>
        /// Sends a chat message to all connected clients.
        /// </summary>
        /// <param name="message">The message text.</param>
        /// <param name="prefix">The prefix for the message.</param>
        /// <param name="name">The name to attach to the message.</param>
        public static void SendToAll(string message, string prefix = "SM", string name = null)
        {
            foreach (var client in ConnectionManager.Instance.Clients.List)
            {
                SendTo(client, message, prefix, name);
            }
        }
    }
}
