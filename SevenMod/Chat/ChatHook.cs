// <copyright file="ChatHook.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    using System;
    using System.Collections.Generic;
    using SevenMod.ConVar;
    using SevenMod.Core;
    using SevenMod.Logging;

    /// <summary>
    /// Hooks into the game chat and allows plugins to register their own chat hooks.
    /// </summary>
    public class ChatHook
    {
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
        /// Occurs when a chat message is received.
        /// </summary>
        public static event EventHandler<ChatMessageEventArgs> ChatMessage;

        /// <summary>
        /// Initializes the chat hook system.
        /// </summary>
        internal static void Init()
        {
            publicChatTrigger = ConVarManager.CreateConVar(null, "PublicChatTrigger", "!", "Chat prefix for public commands.").Value;
            silentChatTrigger = ConVarManager.CreateConVar(null, "SilentChatTrigger", "/", "Chat prefix for silent commands.").Value;
        }

        /// <summary>
        /// Processes a chat message.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that sent the message.</param>
        /// <param name="type">The type of chat message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="recipientEntityIds">The list of entity IDs receiving the message.</param>
        /// <returns><c>true</c> to allow the message to continue propagating; <c>false</c> to consume the message.</returns>
        internal static bool HookChatMessage(ClientInfo client, EChatType type, string message, List<int> recipientEntityIds)
        {
            if (client == null)
            {
                return true;
            }

            if (ChatMessage != null)
            {
                var args = new ChatMessageEventArgs(client, type, message, recipientEntityIds);
                foreach (EventHandler<ChatMessageEventArgs> d in ChatMessage.GetInvocationList())
                {
                    try
                    {
                        d.Invoke(null, args);
                        if (args.Handled)
                        {
                            return false;
                        }
                    }
                    catch (HaltPluginException)
                    {
                        args.Handled = false;
                    }
                    catch (Exception e)
                    {
                        args.Handled = false;
                        if (d.Target is IPlugin p)
                        {
                            p.Container.SetFailState(e);
                        }
                        else
                        {
                            SMLog.Error(e);
                        }
                    }
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
        internal static bool ShouldReplyToChat(ClientInfo client)
        {
            return client != null && replyToChat.Contains(client.entityId);
        }
    }
}
