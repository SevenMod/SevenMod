// <copyright file="AdminManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Admin
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages admin users of the mod.
    /// </summary>
    public class AdminManager
    {
        /// <summary>
        /// Maps admin auth IDs to their <see cref="AdminInfo"/> objects.
        /// </summary>
        private static Dictionary<string, AdminInfo> admins = new Dictionary<string, AdminInfo>();

        /// <summary>
        /// Maps admin access flag characters to the associated <see cref="AdminFlags"/> value.
        /// </summary>
        private static Dictionary<char, AdminFlags> adminFlagKeys = new Dictionary<char, AdminFlags>()
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
        /// Gets a copy of the <see cref="admins"/>.
        /// </summary>
        public static Dictionary<string, AdminInfo> Admins
        {
            get => new Dictionary<string, AdminInfo>(admins);
        }

        /// <summary>
        /// Gets a copy of the <see cref="adminFlagKeys"/>.
        /// </summary>
        public static Dictionary<char, AdminFlags> AdminFlagKeys
        {
            get => new Dictionary<char, AdminFlags>(adminFlagKeys);
        }

        /// <summary>
        /// Adds an admin user.
        /// </summary>
        /// <param name="authId">The user's 64 bit auth ID.</param>
        /// <param name="immunity">The user's immunity level.</param>
        /// <param name="flags">The user's access flag string.</param>
        public static void AddAdmin(string authId, int immunity, string flags)
        {
            var admin = new AdminInfo(authId, immunity, ParseFlags(flags));
            if (admins.ContainsKey(authId))
            {
                immunity = Math.Max(immunity, admins[authId].Immunity);
                var combinedFlags = admin.Flags | admins[authId].Flags;
                admin = new AdminInfo(authId, immunity, combinedFlags);
            }

            admins[authId] = admin;
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
        /// Removes all admin users.
        /// </summary>
        public static void RemoveAllAdmins()
        {
            admins.Clear();
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
        /// Gets the <see cref="AdminInfo"/> for an admin user.
        /// </summary>
        /// <param name="authId">The 64 bit auth ID of the user to check.</param>
        /// <returns>The <see cref="AdminInfo"/> for the admin user.</returns>
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
        /// <param name="client">The <see cref="ClientInfo"/> representing the client.</param>
        /// <param name="adminFlag">The <see cref="AdminFlags"/> against which to match.</param>
        /// <returns><c>true</c> if the user has access to the specified flags; otherwise
        /// <c>false</c>.</returns>
        public static bool CheckAccess(ClientInfo client, AdminFlags adminFlag)
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
        /// <param name="client">The <see cref="ClientInfo"/> representing the source
        /// client.</param>
        /// <param name="target">The <see cref="ClientInfo"/> representing the target
        /// client.</param>
        /// <returns><c>true</c> if <paramref name="client"/> can target <paramref name="target"/>;
        /// otherwise <c>false</c>.</returns>
        public static bool CanTarget(ClientInfo client, ClientInfo target)
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
        protected static AdminFlags ParseFlags(string flagString)
        {
            AdminFlags flags = 0;

            foreach (var f in flagString)
            {
                if (adminFlagKeys.ContainsKey(f))
                {
                    flags |= adminFlagKeys[f];
                }
            }

            return flags;
        }
    }
}
