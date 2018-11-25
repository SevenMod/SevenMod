// <copyright file="ChatHook.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    using System.Collections.Generic;
    using SevenMod.ConVar;

    /// <summary>
    /// Hooks into the game chat and allows plugins to register their own chat hooks.
    /// </summary>
    public class ChatHook
    {
        /// <summary>
        /// <see cref="IChatHookListener"/> objects registered by plugins.
        /// </summary>
        private static List<IChatHookListener> listeners = new List<IChatHookListener>();

        /// <summary>
        /// Client entity IDs for which commands should reply to via game chat.
        /// </summary>
        private static List<int> replyToChat = new List<int>();

        /// <summary>
        /// The value of the PublicChatTrigger <see cref="ConVar"/>.
        /// </summary>
        private static ConVarValue publicChatTrigger;

        /// <summary>
        /// The value of the SilentChatTrigger <see cref="ConVar"/>.
        /// </summary>
        private static ConVarValue silentChatTrigger;

        /// <summary>
        /// Initializes the chat hook system.
        /// </summary>
        public static void Init()
        {
            publicChatTrigger = ConVarManager.CreateConVar("PublicChatTrigger", "!", "Chat prefix for public commands.").Value;
            silentChatTrigger = ConVarManager.CreateConVar("SilentChatTrigger", "/", "Chat prefix for silent commands.").Value;
        }

        /// <summary>
        /// Processes a chat message.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="message">The message text.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to consume the message.</returns>
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

            if (message.StartsWith(publicChatTrigger.AsString) || message.StartsWith(silentChatTrigger.AsString))
            {
                replyToChat.Add(client.entityId);
                ConnectionManager.Instance.ServerConsoleCommand(client, $"sm {message.Substring(1)}");
                replyToChat.Remove(client.entityId);

                return message.StartsWith(publicChatTrigger.AsString);
            }

            return true;
        }

        /// <summary>
        /// Checks whether commands should reply to a user via chat.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client for which to reply.</param>
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
