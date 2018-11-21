// <copyright file="ReservedSlotsConfig.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.ReservedSlots
{
    /// <summary>
    /// Represents the configuration for the ReservedSlots plugin.
    /// </summary>
    public class ReservedSlotsConfig
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="ReservedSlotsConfig"/> class.
        /// </summary>
        public static ReservedSlotsConfig Instance { get; } = new ReservedSlotsConfig();

        /// <summary>
        /// Gets or sets the number of reserved slots.
        /// </summary>
        public int ReservedSlots { get; set; } = 0;
    }
}
