// <copyright file="SMConsoleHelper.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
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
        /// <param name="client">The <see cref="ClientInfo"/> object representing the source client.</param>
        /// <param name="targetString">The player target string.</param>
        /// <param name="targets">Will be set to a list of <see cref="SMClient"/> objects representing the matching clients.</param>
        /// <param name="targetName">Will be set to the name of the target, if applicable.</param>
        /// <param name="nameIsPhrase">Will be set to a value indicating whether <paramref name="targetName"/> is a translation phrase.</param>
        /// <returns>The number of matching clients.</returns>
        public static int ParseTargetString(ClientInfo client, string targetString, out List<SMClient> targets, out string targetName, out bool nameIsPhrase)
        {
            var list = new List<ClientInfo>();
            targetName = null;
            nameIsPhrase = false;

            if (targetString.StartsWith("@"))
            {
                switch (targetString.Substring(1).ToLower())
                {
                    case "all":
                        list.AddRange(ConnectionManager.Instance.Clients.List);
                        targetName = "All players";
                        nameIsPhrase = true;
                        break;
                    case "me":
                        if (client == null)
                        {
                            ChatHelper.ReplyToCommand(client, "Server cannot target self");
                            break;
                        }

                        list.Add(client);
                        break;
                    case "!me":
                        list.AddRange(ConnectionManager.Instance.Clients.List);
                        list.Remove(client);
                        targetName = "All players";
                        nameIsPhrase = true;
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
                    ChatHelper.ReplyToCommand(client, "No valid targets found");
                }
            }
            else
            {
                if (ParseSingleTargetString(client, targetString, out var target))
                {
                    list.Add(target.ClientInfo);
                }
            }

            foreach (var target in list)
            {
                if (!AdminManager.CanTarget(client, target))
                {
                    ChatHelper.ReplyToCommand(client, $"Cannot target {target.playerName}");
                }
            }

            list.RemoveAll((ClientInfo target) => !AdminManager.CanTarget(client, target));

            if (!nameIsPhrase && list.Count == 1)
            {
                targetName = list[0].playerName;
            }

            targets = list.ConvertAll(e => new SMClient(e));
            return targets.Count;
        }

        /// <summary>
        /// Parse a single player target string into a connected client.
        /// </summary>
        /// <param name="client">The calling client information.</param>
        /// <param name="targetString">The player target string.</param>
        /// <param name="target">Variable to be set to the <see cref="SMClient"/> object representing the matching client.</param>
        /// <returns><c>true</c> if a match is found; otherwise <c>false</c>.</returns>
        public static bool ParseSingleTargetString(ClientInfo client, string targetString, out SMClient target)
        {
            var count = ConsoleHelper.ParseParamPartialNameOrId(targetString, out var id, out var clientInfo, false);
            if (count < 1)
            {
                ChatHelper.ReplyToCommand(client, "No valid targets found");
                target = null;
                return false;
            }
            else if (count > 1)
            {
                ChatHelper.ReplyToCommand(client, "Multiple valid targets found");
                target = null;
                return false;
            }

            target = new SMClient(clientInfo);
            return true;
        }
    }
}
