// <copyright file="BaseVotesConfig.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    /// <summary>
    /// Represents the configuration for the BaseVotes plugin.
    /// </summary>
    public class BaseVotesConfig
    {
        /// <summary>
        /// Gets the singleton instance of the <see cref="BaseVotesConfig"/> class.
        /// </summary>
        public static BaseVotesConfig Instance { get; } = new BaseVotesConfig();

        /// <summary>
        /// Gets or sets the percentage of votes required for a ban vote to succeed.
        /// </summary>
        public float VoteBanPercent { get; set; } = 0.60f;

        /// <summary>
        /// Gets or sets the percentage of votes required for a kick vote to succeed.
        /// </summary>
        public float VoteKickPercent { get; set; } = 0.60f;
    }
}
