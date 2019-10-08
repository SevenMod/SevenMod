// <copyright file="ChatHelper.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    using System.Text;
    using SevenMod.Admin;
    using SevenMod.ConVar;
    using SevenMod.Core;
    using SevenMod.Lang;

    /// <summary>
    /// Chat related utilities.
    /// </summary>
    internal class ChatHelper
    {
        /// <summary>
        /// The value of the ShowActivity <see cref="ConVar"/>.
        /// </summary>
        private static ConVarValue showActivity;

        /// <summary>
        /// Initializes the chat helper.
        /// </summary>
        public static void Init()
        {
            var description = new StringBuilder()
                .AppendLine("Specifies how admin activity should be relayed to users. Add up the values below to get the functionality you want.")
                .AppendLine("1: Show admin activity to non-admins anonymously.")
                .AppendLine("2: If 1 is specified, admin names will be shown.")
                .AppendLine("4: Show admin activity to admins anonymously.")
                .AppendLine("8: If 4 is specified, admin names will be shown.")
                .AppendLine("16: Always show admin names to root users.")
                .AppendLine()
                .AppendLine("Default: 13 (1+4+8)")
                .ToString();

            showActivity = ConVarManager.CreateConVar(null, "ShowActivity", "13", description, true, 0, true, 31).Value;
        }

        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the client used to call the currently executing command.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing calling client information.</param>
        /// <param name="message">The language key or message to send.</param>
        /// <param name="args">The parameters for the language format string.</param>
        public static void ReplyToCommand(ClientInfo client, string message, params object[] args)
        {
            if ((client != null) && ChatHook.ShouldReplyToChat(client))
            {
                SendTo(client, $"[\u200BSM] {message}", args);
            }
            else
            {
                SdtdConsole.Instance.Output("[SM] " + Language.GetString(message, client, args));
            }
        }

        /// <summary>
        /// Sends a chat message to an individual client.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client.</param>
        /// <param name="message">The language key or message to send.</param>
        /// <param name="args">The parameters for the language format string.</param>
        public static void SendTo(ClientInfo client, string message, params object[] args)
        {
            if (client == null || client == SMClient.Console.ClientInfo)
            {
                return;
            }

            message = Language.GetString(message, client, args);

            var package = new NetPackageChat();
            package.Setup(EChatType.Global, 0, message, null, false, null);
            client.SendPackage(package);
        }

        /// <summary>
        /// Sends a chat message to all connected clients.
        /// </summary>
        /// <param name="message">The language key or message to send.</param>
        /// <param name="args">The parameters for the language format string.</param>
        public static void SendToAll(string message, params object[] args)
        {
            foreach (var client in ConnectionManager.Instance.Clients.List)
            {
                SendTo(client, message, args);
            }
        }

        /// <summary>
        /// Show admin activity based on the rules set by the ShowActivity <see cref="ConVar"/>.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client performing the action.</param>
        /// <param name="message">The language key or message to send.</param>
        /// <param name="args">The parameters for the language format string.</param>
        public static void ShowActivity(ClientInfo client, string message, params object[] args)
        {
            var show = showActivity.AsInt;
            var tag = AdminManager.CheckAccess(client, AdminFlags.Generic) ? "ADMIN" : "PLAYER";
            var name = client == null ? "Console" : client.playerName;
            foreach (var c in ConnectionManager.Instance.Clients.List)
            {
                var translated = Language.GetString(message, c, args);
                if (c == client)
                {
                    SendTo(c, $"[\u200BSM] {translated}");
                    continue;
                }

                if ((show & 1) == 1)
                {
                    string actualTag;
                    if ((show & 2) == 2 || ((show & 8) == 8 && AdminManager.CheckAccess(c, AdminFlags.Generic)) || ((show & 16) == 16 && AdminManager.CheckAccess(c, AdminFlags.Root)))
                    {
                        actualTag = name;
                    }
                    else
                    {
                        actualTag = Language.GetString(tag, c);
                    }

                    SendTo(c, $"[\u200BSM] {actualTag}: {translated}");
                }
                else if ((show & 4) == 4 && AdminManager.CheckAccess(c, AdminFlags.Generic))
                {
                    string actualTag;
                    if ((show & 8) == 8 || ((show & 16) == 16 && AdminManager.CheckAccess(c, AdminFlags.Root)))
                    {
                        actualTag = name;
                    }
                    else
                    {
                        actualTag = Language.GetString(tag, c);
                    }

                    SendTo(c, $"[\u200BSM] {actualTag}: {translated}");
                }
            }
        }
    }
}
