// <copyright file="PluginInfo.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Structure for containing metadata for a plugin.
    /// </summary>
    public struct PluginInfo
    {
        /// <summary>
        /// An empty instance of the <see cref="PluginInfo"/> struct.
        /// </summary>
        public static readonly PluginInfo Empty = new PluginInfo
        {
            Name = string.Empty,
            Author = string.Empty,
            Description = string.Empty,
            Version = "0.0.0.0",
            Website = string.Empty,
        };

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public string Name;

        /// <summary>
        /// The name of the plugin author(s).
        /// </summary>
        public string Author;

        /// <summary>
        /// A brief description for the plugin.
        /// </summary>
        public string Description;

        /// <summary>
        /// The version identifier.
        /// </summary>
        public string Version;

        /// <summary>
        /// The website associated with the plugin.
        /// </summary>
        public string Website;
    }
}
