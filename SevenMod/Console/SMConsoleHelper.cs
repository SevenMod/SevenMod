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
    /// Represents an admin command, a special console command that is managed by the mod, with
    /// built in permission checking.
    /// </summary>
    public class SMConsoleHelper
    {
        /// <summary>
        /// Parse a player target string into a list of currently connected clients.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <returns>A list of <see cref="ClientInfo"/> objects representing the matching
        /// clients.</returns>
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

                if (SteamUtils.NormalizeSteamId(exactMatch, out string steamId))
                {
                    list.Add(ConnectionManager.Instance.Clients.ForPlayerId(steamId));
                }
                else if (int.TryParse(exactMatch, out int entId))
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
                var target = ParseSingleTargetString(senderInfo, targetString);
                if (target != null)
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
        /// Parse a single player target string into a <see cref="ClientInfo"/> object representing
        /// a currently connected client, or <c>null</c> if no unique target is found.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <returns>A <see cref="ClientInfo"/> object representing a matching client if one is
        /// found; otherwise <c>null</c>.</returns>
        public static ClientInfo ParseSingleTargetString(CommandSenderInfo senderInfo, string targetString)
        {
            var count = global::ConsoleHelper.ParseParamPartialNameOrId(targetString, out string _, out ClientInfo target, false);
            if (count < 1 || (target == null))
            {
                ChatHelper.ReplyToCommand(senderInfo, "No valid targets found");
            }
            else if (count > 1)
            {
                ChatHelper.ReplyToCommand(senderInfo, "Multiple valid targets found");
            }

            return target;
        }
    }
}
