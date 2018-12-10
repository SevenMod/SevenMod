// <copyright file="SMClient.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;

    /// <summary>
    /// Represents a client on the server.
    /// </summary>
    public class SMClient : IEquatable<SMClient>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SMClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> representing the client.</param>
        internal SMClient(ClientInfo client)
        {
            this.Ping = client.ping;
            this.LoginDone = client.loginDone;
            this.PlayerId = client.playerId;
            this.EntityId = client.entityId;
            this.PlayerName = client.playerName;
            this.Ip = client.ip;
            this.ClientInfo = client;
        }

        /// <summary>
        /// Gets the ping of the client.
        /// </summary>
        public int Ping { get; }

        /// <summary>
        /// Gets a value indicating whether the client has logged in.
        /// </summary>
        public bool LoginDone { get; }

        /// <summary>
        /// Gets the player ID (SteamID64) of the client.
        /// </summary>
        public string PlayerId { get; }

        /// <summary>
        /// Gets the entity ID of the client.
        /// </summary>
        public int EntityId { get; }

        /// <summary>
        /// Gets the player name of the client.
        /// </summary>
        public string PlayerName { get; }

        /// <summary>
        /// Gets the IP address of the client.
        /// </summary>
        public string Ip { get; }

        /// <summary>
        /// Gets the original <see cref="ClientInfo"/> object containing more detailed information.
        /// </summary>
        internal ClientInfo ClientInfo { get; }

        /// <inheritdoc/>
        public bool Equals(SMClient other)
        {
            return this.EntityId == other.EntityId;
        }
    }
}
