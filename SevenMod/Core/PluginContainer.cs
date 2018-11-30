// <copyright file="PluginContainer.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Contains a plugin with its metadata
    /// </summary>
    public class PluginContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginContainer"/> class.
        /// </summary>
        /// <param name="file">The name of the plugin binary file.</param>
        internal PluginContainer(string file)
        {
            this.File = file;
        }

        /// <summary>
        /// Enumeration of plugin load statuses.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Represents the status of a plugin that is not currently loaded.
            /// </summary>
            Unloaded,

            /// <summary>
            /// Represents the status of a plugin that is currently loaded.
            /// </summary>
            Loaded,

            /// <summary>
            /// Represents the status of a plugin that is currently in an error state.
            /// </summary>
            Error,
        }

        /// <summary>
        /// Gets the name of the plugin binary file.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets the current status of the plugin.
        /// </summary>
        public Status LoadStatus { get; internal set; } = Status.Unloaded;

        /// <summary>
        /// Gets the error message for the plugin if it is currently in an error state.
        /// </summary>
        public string Error { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the <see cref="PluginInfo"/> structure containing the plugin metadata.
        /// </summary>
        public PluginInfo PluginInfo { get; internal set; } = PluginInfo.Empty;

        /// <summary>
        /// Gets or sets the <see cref="IPlugin"/> object representing the plugin.
        /// </summary>
        internal IPlugin Plugin { get; set; }
    }
}
