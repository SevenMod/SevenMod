// <copyright file="ClientHelper.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Utility methods for dealing with clients.
    /// </summary>
    public class ClientHelper
    {
        /// <summary>
        /// Gets the number of clients connected to the server.
        /// </summary>
        public static int ClientCount { get => ConnectionManager.Instance.ClientCount(); }

        /// <summary>
        /// Gets a list of <see cref="SMClient"/> objects representing the clients connected to the server.
        /// </summary>
        public static ReadOnlyCollection<SMClient> List { get => new List<ClientInfo>(ConnectionManager.Instance.Clients.List).ConvertAll(e => new SMClient(e)).AsReadOnly(); }

        /// <summary>
        /// Gets the client with the specified entity ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <returns>A <see cref="SMClient"/> object representing the matching client; or <c>null</c> if none is found.</returns>
        public static SMClient ForEntityId(int entityId)
        {
            var client = ConnectionManager.Instance.Clients.ForEntityId(entityId);
            return (client == null) ? null : new SMClient(client);
        }

        /// <summary>
        /// Gets the client with the specified player ID.
        /// </summary>
        /// <param name="playerId">The player ID.</param>
        /// <returns>A <see cref="SMClient"/> object representing the matching client; or <c>null</c> if none is found.</returns>
        public static SMClient ForPlayerId(string playerId)
        {
            var client = ConnectionManager.Instance.Clients.ForPlayerId(playerId);
            return (client == null) ? null : new SMClient(client);
        }

        /// <summary>
        /// Gets the client with the specified name or player ID.
        /// </summary>
        /// <param name="nameOrId">The name or player ID.</param>
        /// <param name="ignoreCase">A value indicating whether to ignore case.</param>
        /// <param name="ignoreBlanks">A value indicating whether to ignore spaces.</param>
        /// <returns>A <see cref="SMClient"/> object representing the matching client; or <c>null</c> if none is found.</returns>
        public static SMClient GetForNameOrId(string nameOrId, bool ignoreCase = true, bool ignoreBlanks = false)
        {
            var client = ConnectionManager.Instance.Clients.GetForNameOrId(nameOrId, ignoreCase, ignoreBlanks);
            return (client == null) ? null : new SMClient(client);
        }

        /// <summary>
        /// Gets the client with the specified name.
        /// </summary>
        /// <param name="playerName">The name.</param>
        /// <param name="ignoreCase">A value indicating whether to ignore case.</param>
        /// <param name="ignoreBlanks">A value indicating whether to ignore spaces.</param>
        /// <returns>A <see cref="SMClient"/> object representing the matching client; or <c>null</c> if none is found.</returns>
        public static SMClient GetForPlayerName(string playerName, bool ignoreCase = true, bool ignoreBlanks = false)
        {
            var client = ConnectionManager.Instance.Clients.GetForPlayerName(playerName, ignoreCase, ignoreBlanks);
            return (client == null) ? null : new SMClient(client);
        }
    }
}
