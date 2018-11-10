// <copyright file="BaseVotes.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: BaseVotes</para>
    /// <para>Adds the vote, votekick, and voteban admin commands.</para>
    /// </summary>
    public class BaseVotes : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Votes",
            Author = "SevenMod",
            Description = "Adds basic voting commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            ConfigManager.ParseConfig(BaseVotesConfig.Instance, "BaseVotes");

            this.RegAdminCmd("vote", new AdminCmdVote(), AdminFlags.Vote);
            this.RegAdminCmd("voteban", new AdminCmdVoteBan(), AdminFlags.Vote);
            this.RegAdminCmd("votekick", new AdminCmdVoteKick(), AdminFlags.Vote);
        }
    }
}
