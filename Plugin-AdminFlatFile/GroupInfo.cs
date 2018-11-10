// <copyright file="GroupInfo.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.AdminFlatFile
{
    /// <summary>
    /// Represents an admin group.
    /// </summary>
    internal class GroupInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="immunity">The immunity level of the group.</param>
        /// <param name="flags">The access flag string for the group.</param>
        internal GroupInfo(string name, int immunity, string flags)
        {
            this.Name = name;
            this.Immunity = immunity;
            this.Flags = flags;
        }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the immunity level of the group.
        /// </summary>
        public int Immunity { get; }

        /// <summary>
        /// Gets the access flag string for the group.
        /// </summary>
        public string Flags { get; }
    }
}
