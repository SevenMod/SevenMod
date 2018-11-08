// <copyright file="AdminInfo.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Represents an admin user.
    /// </summary>
    public class AdminInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminInfo"/> class.
        /// </summary>
        /// <param name="authId">The 64 bit AuthID of the user.</param>
        /// <param name="immunity">The user's immunity level.</param>
        /// <param name="flags">The user's access flags.</param>
        internal AdminInfo(string authId, int immunity, AdminFlags flags)
        {
            this.AuthID = authId;
            this.Immunity = immunity;
            this.Flags = flags;
        }

        /// <summary>
        /// Gets the 64 bit AuthID of the user.
        /// </summary>
        public string AuthID { get; }

        /// <summary>
        /// Gets the user's immunity level. Users with lower values cannot target users with higher
        /// values.
        /// </summary>
        public int Immunity { get; }

        /// <summary>
        /// Gets the user's access flags.
        /// </summary>
        public AdminFlags Flags { get; }
    }
}
