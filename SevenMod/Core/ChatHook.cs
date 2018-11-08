// <copyright file="ChatHook.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Hooks into the game chat and allows plugins to register their own chat hooks.
    /// </summary>
    public class ChatHook
    {
        /// <summary>
        /// <see cref="IChatHookListener"/>s registered by plugins.
        /// </summary>
        private static List<IChatHookListener> listeners = new List<IChatHookListener>();

        /// <summary>
        /// Client entity IDs for which commands should reply to via game chat.
        /// </summary>
        private static List<int> replyToChat = new List<int>();

        /// <summary>
        /// Processes a chat message.
        /// </summary>
        /// <param name="client">The client that sent the message.</param>
        /// <param name="message">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to
        /// consume the message.</returns>
        public static bool HookChatMessage(ClientInfo client, string message)
        {
            if (client == null)
            {
                return true;
            }

            foreach (var listener in listeners)
            {
                if (!listener.OnChatMessage(client, message))
                {
                    return false;
                }
            }

            if (message.StartsWith(CoreConfig.Instance.PublicChatTrigger) || message.StartsWith(CoreConfig.Instance.SilentChatTrigger))
            {
                replyToChat.Add(client.entityId);
                ConnectionManager.Instance.ServerConsoleCommand(client, "sm_" + message.Substring(1));
                replyToChat.Remove(client.entityId);

                return message.StartsWith(CoreConfig.Instance.PublicChatTrigger);
            }

            return true;
        }

        /// <summary>
        /// Checks whether commands should reply to a user via chat.
        /// </summary>
        /// <param name="client">The client for which to reply.</param>
        /// <returns><c>true</c> to reply via chat; <c>false</c> to reply via console.</returns>
        public static bool ShouldReplyToChat(ClientInfo client)
        {
            return replyToChat.Contains(client.entityId);
        }

        /// <summary>
        /// Registers a custom chat hook.
        /// </summary>
        /// <param name="listener">The <see cref="IChatHookListener"/> to register.</param>
        public static void RegisterChatHook(IChatHookListener listener)
        {
            listeners.Add(listener);
        }

        /// <summary>
        /// Unregisters a custom chat hook.
        /// </summary>
        /// <param name="listener">The <see cref="IChatHookListener"/> to unregister.</param>
        public static void UnregisterChatHook(IChatHookListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
