// <copyright file="MOTDConfig.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.MOTD
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the configuration for the MOTD plugin.
    /// </summary>
    public class MOTDConfig
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="MOTDConfig"/> class.
        /// </summary>
        public static MOTDConfig Instance { get; } = new MOTDConfig();

        /// <summary>
        /// Gets or sets the list of messages to print to chat.
        /// </summary>
        public List<string> Lines { get; set; } = new List<string>();
    }
}
