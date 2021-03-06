﻿// <copyright file="AdminManager.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SevenMod.Core;

    /// <summary>
    /// Manages admin users of the mod.
    /// </summary>
    public class AdminManager
    {
        /// <summary>
        /// Maps admin access flag characters to the associated <see cref="AdminFlags"/> value.
        /// </summary>
        private static readonly Dictionary<char, AdminFlags> AdminFlagKeysValue = new Dictionary<char, AdminFlags>()
        {
            { 'a', AdminFlags.Reservation },
            { 'b', AdminFlags.Generic },
            { 'c', AdminFlags.Kick },
            { 'd', AdminFlags.Ban },
            { 'e', AdminFlags.Unban },
            { 'f', AdminFlags.Slay },
            { 'g', AdminFlags.Changemap },
            { 'h', AdminFlags.Convars },
            { 'i', AdminFlags.Config },
            { 'j', AdminFlags.Chat },
            { 'k', AdminFlags.Vote },
            { 'l', AdminFlags.Password },
            { 'm', AdminFlags.RCON },
            { 'n', AdminFlags.Cheats },
            { 'o', AdminFlags.Custom1 },
            { 'p', AdminFlags.Custom2 },
            { 'q', AdminFlags.Custom3 },
            { 'r', AdminFlags.Custom4 },
            { 's', AdminFlags.Custom5 },
            { 't', AdminFlags.Custom6 },
            { 'z', AdminFlags.Root },
        };

        /// <summary>
        /// Maps admin auth IDs to their <see cref="AdminInfo"/> objects.
        /// </summary>
        private static Dictionary<string, AdminInfo> admins = new Dictionary<string, AdminInfo>
        {
            { "-1", new AdminInfo("-1", 0, AdminFlags.Root) },
        };

        /// <summary>
        /// Gets the map of admin access flag characters to the associated <see cref="AdminFlags"/> value.
        /// </summary>
        public static Dictionary<char, AdminFlags> AdminFlagKeys
        {
            get => new Dictionary<char, AdminFlags>(AdminFlagKeysValue);
        }

        /// <summary>
        /// Adds an admin user.
        /// </summary>
        /// <param name="authId">The user's 64 bit auth ID.</param>
        /// <param name="immunity">The user's immunity level.</param>
        /// <param name="flags">The user's access flag string.</param>
        public static void AddAdmin(string authId, int immunity, string flags)
        {
            if (!SteamUtils.NormalizeSteamId(authId, out var steamId64))
            {
                return;
            }

            var admin = new AdminInfo(steamId64, immunity, ParseFlags(flags));
            if (admins.ContainsKey(steamId64))
            {
                immunity = Math.Max(immunity, admins[steamId64].Immunity);
                var combinedFlags = admin.Flags | admins[steamId64].Flags;
                admin = new AdminInfo(steamId64, immunity, combinedFlags);
            }

            admins[steamId64] = admin;
        }

        /// <summary>
        /// Removes an admin user.
        /// </summary>
        /// <param name="authId">The user's 64 bit auth ID.</param>
        public static void RemoveAdmin(string authId)
        {
            admins.Remove(authId);
        }

        /// <summary>
        /// Triggers a reload of the admin list.
        /// </summary>
        public static void ReloadAdmins()
        {
            var console = admins["-1"];
            admins.Clear();
            admins["-1"] = console;

            foreach (var k in PluginManager.Plugins.Keys.ToArray())
            {
                if (PluginManager.Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    try
                    {
                        plugin.Plugin.OnReloadAdmins();
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether a user is an admin user.
        /// </summary>
        /// <param name="authId">The 64 bit auth ID of the user to check.</param>
        /// <returns><c>true</c> if the user is an admin user; otherwise <c>false</c>.</returns>
        public static bool IsAdmin(string authId)
        {
            return admins.ContainsKey(authId);
        }

        /// <summary>
        /// Gets the <see cref="AdminInfo"/> object for an admin user.
        /// </summary>
        /// <param name="authId">The 64 bit auth ID of the user to locate.</param>
        /// <returns>The <see cref="AdminInfo"/> object for the admin user.</returns>
        public static AdminInfo GetAdmin(string authId)
        {
            if (admins.ContainsKey(authId))
            {
                return admins[authId];
            }

            return new AdminInfo(authId, 0, 0);
        }

        /// <summary>
        /// Checks whether a client has a specific set of admin access flags.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the client.</param>
        /// <param name="adminFlag">The <see cref="AdminFlags"/> value against which to match the client.</param>
        /// <returns><c>true</c> if the user has access to the specified flags; otherwise <c>false</c>.</returns>
        public static bool CheckAccess(SMClient client, AdminFlags adminFlag)
        {
            return CheckAccess(client?.ClientInfo, adminFlag);
        }

        /// <summary>
        /// Checks whether a client is allowed to target a another specific client.
        /// </summary>
        /// <param name="client">The <see cref="SMClient"/> object representing the source client.</param>
        /// <param name="target">The <see cref="SMClient"/> object representing the target client.</param>
        /// <returns><c>true</c> if <paramref name="client"/> can target <paramref name="target"/>; otherwise <c>false</c>.</returns>
        public static bool CanTarget(SMClient client, SMClient target)
        {
            return CanTarget(client?.ClientInfo, target.ClientInfo);
        }

        /// <summary>
        /// Checks whether a client has a specific set of admin access flags.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client.</param>
        /// <param name="adminFlag">The <see cref="AdminFlags"/> value against which to match the client.</param>
        /// <returns><c>true</c> if the user has access to the specified flags; otherwise <c>false</c>.</returns>
        internal static bool CheckAccess(ClientInfo client, AdminFlags adminFlag)
        {
            if (client == null)
            {
                return true;
            }

            if (!admins.ContainsKey(client.playerId))
            {
                return false;
            }

            var flags = admins[client.playerId].Flags;
            if (((flags & adminFlag) == adminFlag) || ((flags & AdminFlags.Root) == AdminFlags.Root))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether a client is allowed to target a another specific client.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the source client.</param>
        /// <param name="target">The <see cref="ClientInfo"/> object representing the target client.</param>
        /// <returns><c>true</c> if <paramref name="client"/> can target <paramref name="target"/>; otherwise <c>false</c>.</returns>
        internal static bool CanTarget(ClientInfo client, ClientInfo target)
        {
            if (client == null)
            {
                return true;
            }

            var clientImmunity = 0;
            if (admins.ContainsKey(client.playerId))
            {
                if ((admins[client.playerId].Flags & AdminFlags.Root) == AdminFlags.Root)
                {
                    return true;
                }

                clientImmunity = admins[client.playerId].Immunity;
            }

            var targetImmunity = 0;
            if (admins.ContainsKey(target.playerId))
            {
                targetImmunity = admins[target.playerId].Immunity;
            }

            return clientImmunity >= targetImmunity;
        }

        /// <summary>
        /// Gets the <see cref="AdminFlags"/> value from a string of access flag characters.
        /// </summary>
        /// <param name="flagString">A string of access flag characters.</param>
        /// <returns>The <see cref="AdminFlags"/> representing the access flags.</returns>
        private static AdminFlags ParseFlags(string flagString)
        {
            AdminFlags flags = 0;

            foreach (var f in flagString)
            {
                if (AdminFlagKeysValue.ContainsKey(f))
                {
                    flags |= AdminFlagKeysValue[f];
                }
            }

            return flags;
        }
    }
}
