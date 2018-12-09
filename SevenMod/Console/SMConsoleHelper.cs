// <copyright file="SMConsoleHelper.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System.Collections.Generic;
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// Utility methods for interacting with console commands.
    /// </summary>
    internal class SMConsoleHelper
    {
        /// <summary>
        /// Parse a player target string into a list of currently connected clients.
        /// </summary>
        /// <param name="senderInfo">The <see cref="CommandSenderInfo"/> object representing the source client.</param>
        /// <param name="targetString">The player target string.</param>
        /// <returns>A list of <see cref="ClientInfo"/> objects representing the matching clients.</returns>
        public static List<ClientInfo> ParseTargetString(CommandSenderInfo senderInfo, string targetString)
        {
            var list = new List<ClientInfo>();

            if (targetString.StartsWith("@"))
            {
                switch (targetString.Substring(1).ToLower())
                {
                    case "all":
                        list.AddRange(ConnectionManager.Instance.Clients.List);
                        break;
                    case "me":
                        if (senderInfo.RemoteClientInfo == null)
                        {
                            ChatHelper.ReplyToCommand(senderInfo, "Server cannot target self");
                            break;
                        }

                        list.Add(senderInfo.RemoteClientInfo);
                        break;
                    case "!me":
                        list.AddRange(ConnectionManager.Instance.Clients.List);
                        list.Remove(senderInfo.RemoteClientInfo);
                        break;
                }
            }
            else if (targetString.StartsWith("#"))
            {
                var exactMatch = targetString.Substring(1);
                if (exactMatch.Substring(0, 6).EqualsCaseInsensitive("STEAM_"))
                {
                    exactMatch = $"STEAM_{exactMatch.Substring(6).Replace('_', ':')}";
                }

                if (SteamUtils.NormalizeSteamId(exactMatch, out var steamId))
                {
                    list.Add(ConnectionManager.Instance.Clients.ForPlayerId(steamId));
                }
                else if (int.TryParse(exactMatch, out var entId))
                {
                    list.Add(ConnectionManager.Instance.Clients.ForEntityId(entId));
                }
                else
                {
                    list.Add(ConnectionManager.Instance.Clients.GetForPlayerName(exactMatch, false));
                    if (list.Count == 0)
                    {
                        list.Add(ConnectionManager.Instance.Clients.GetForPlayerName(exactMatch));
                    }
                }

                if (list.Count == 0)
                {
                    ChatHelper.ReplyToCommand(senderInfo, "No valid targets found");
                }
            }
            else
            {
                if (ParseSingleTargetString(senderInfo, targetString, out var target))
                {
                    list.Add(target);
                }
            }

            foreach (var client in list)
            {
                if (!AdminManager.CanTarget(senderInfo.RemoteClientInfo, client))
                {
                    ChatHelper.ReplyToCommand(senderInfo, $"Cannot target {client.playerName}");
                    list.Remove(client);
                }
            }

            return list;
        }

        /// <summary>
        /// Parse a single player target string into a connected client.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <param name="client">Variable to be set to the <see cref="ClientInfo"/> object representing the matching client.</param>
        /// <returns><c>true</c> if a match is found; otherwise <c>false</c>.</returns>
        public static bool ParseSingleTargetString(CommandSenderInfo senderInfo, string targetString, out ClientInfo client)
        {
            var count = ConsoleHelper.ParseParamPartialNameOrId(targetString, out var id, out client, false);
            if (count < 1)
            {
                ChatHelper.ReplyToCommand(senderInfo, "No valid targets found");
                return false;
            }
            else if (count > 1)
            {
                ChatHelper.ReplyToCommand(senderInfo, "Multiple valid targets found");
                return false;
            }

            return true;
        }
    }
}
