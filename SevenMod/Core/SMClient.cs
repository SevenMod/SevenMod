// <copyright file="SMClient.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Represents a client on the server.
    /// </summary>
    public class SMClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SMClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> representing the client.</param>
        internal SMClient(ClientInfo client)
        {
            this.ClientInfo = client;
        }

        /// <summary>
        /// Gets the <see cref="SMClient"/> object representing the server console.
        /// </summary>
        public static SMClient Console { get; } = new SMClient(new ClientInfo
        {
            loginDone = true,
            playerId = "-1",
            playerName = "Console",
            ownerId = "-1",
        });

        /// <summary>
        /// Gets the ping of the client.
        /// </summary>
        public int Ping { get => this.ClientInfo.ping; }

        /// <summary>
        /// Gets a value indicating whether the client has logged in.
        /// </summary>
        public bool LoginDone { get => this.ClientInfo.loginDone; }

        /// <summary>
        /// Gets the player ID (SteamID64) of the client.
        /// </summary>
        public string PlayerId { get => this.ClientInfo.playerId; }

        /// <summary>
        /// Gets the entity ID of the client.
        /// </summary>
        public int EntityId { get => this.ClientInfo.entityId; }

        /// <summary>
        /// Gets the player name of the client.
        /// </summary>
        public string PlayerName { get => this.ClientInfo.playerName; }

        /// <summary>
        /// Gets the IP address of the client.
        /// </summary>
        public string Ip { get => this.ClientInfo.ip; }

        /// <summary>
        /// Gets the original <see cref="ClientInfo"/> object containing more detailed information.
        /// </summary>
        internal ClientInfo ClientInfo { get; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A value indicating whether <paramref name="left"/> is equal to <paramref name="right"/>.8</returns>
        public static bool operator ==(SMClient left, SMClient right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>A value indicating whether <paramref name="left"/> is not equal to <paramref name="right"/>.8</returns>
        public static bool operator !=(SMClient left, SMClient right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.ClientInfo.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.ClientInfo.Equals((obj as SMClient)?.ClientInfo);
        }
    }
}
