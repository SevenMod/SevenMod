// <copyright file="AdvertisementsConfig.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.Advertisements
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the configuration for the Advertisements plugin.
    /// </summary>
    public class AdvertisementsConfig
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="AdvertisementsConfig"/> class.
        /// </summary>
        public static AdvertisementsConfig Instance { get; } = new AdvertisementsConfig();

        /// <summary>
        /// Gets or sets the message interval in seconds.
        /// </summary>
        public int Interval { get; set; } = 120;

        /// <summary>
        /// Gets or sets a value indicating whether the messages are displayed in random order.
        /// </summary>
        public bool RandomOrder { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of messages.
        /// </summary>
        public List<string> Messages { get; set; } = new List<string>();
    }
}
