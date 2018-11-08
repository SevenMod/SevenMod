// <copyright file="CoreConfig.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    /// <summary>
    /// Represents the core configuration for the mod.
    /// </summary>
    public class CoreConfig
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="CoreConfig"/> class.
        /// </summary>
        public static CoreConfig Instance { get; } = new CoreConfig();

        /// <summary>
        /// Gets the public chat prefix.
        /// </summary>
        public string PublicChatTrigger { get; internal set; } = "!";

        /// <summary>
        /// Gets the silent chat prefix.
        /// </summary>
        public string SilentChatTrigger { get; internal set; } = "/";
    }
}
