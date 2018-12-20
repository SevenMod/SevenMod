// <copyright file="ChatHelper.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    using System.Text;
    using SevenMod.Admin;
    using SevenMod.ConVar;

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
        /// <param name="message">The message to send.</param>
        /// <param name="prefix">Optional prefix for the message.</param>
        public static void ReplyToCommand(ClientInfo client, string message, string prefix = "SM")
        {
            if ((client != null) && ChatHook.ShouldReplyToChat(client))
            {
                SendTo(client, message, prefix);
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
        /// <param name="prefix">Optional prefix for the message.</param>
        /// <param name="name">Optional name to attach to the message.</param>
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
        /// <param name="prefix">Optional prefix for the message.</param>
        /// <param name="name">Optional name to attach to the message.</param>
        public static void SendToAll(string message, string prefix = "SM", string name = null)
        {
            foreach (var client in ConnectionManager.Instance.Clients.List)
            {
                SendTo(client, message, prefix, name);
            }
        }

        /// <summary>
        /// Show admin activity based on the rules set by the ShowActivity <see cref="ConVar"/>.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client performing the action.</param>
        /// <param name="message">The message text.</param>
        /// <param name="prefix">Optional prefix for the message.</param>
        public static void ShowActivity(ClientInfo client, string message, string prefix = "SM")
        {
            var show = showActivity.AsInt;
            foreach (var c in ConnectionManager.Instance.Clients.List)
            {
                var name = "Admin";
                if ((show & 1) == 1)
                {
                    if ((show & 2) == 2 || ((show & 8) == 8 && AdminManager.CheckAccess(c, AdminFlags.Generic)) || ((show & 16) == 16 && AdminManager.CheckAccess(c, AdminFlags.Root)))
                    {
                        name = client.playerName;
                    }

                    SendTo(c, $"{name} {message}", prefix);
                }
                else if ((show & 4) == 4 && AdminManager.CheckAccess(c, AdminFlags.Generic))
                {
                    if ((show & 8) == 8 || ((show & 16) == 16 && AdminManager.CheckAccess(c, AdminFlags.Root)))
                    {
                        name = client.playerName;
                    }

                    SendTo(c, $"{name} {message}", prefix);
                }
            }
        }
    }
}
